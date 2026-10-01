namespace AtlasOps.ReferenceData.Lifecycle;

using System.Globalization;
using System.Text.Json;

using AtlasOps.ReferenceData.Infrastructure;

public sealed record LifecycleRelease(
    string Product,
    string Cycle,
    DateOnly? ReleaseDate,
    DateOnly? EndOfLife,
    DateOnly? LatestVersionDate,
    string LatestVersion,
    int VersionCount);

public sealed record LifecycleCatalog(
    ReferencePackManifest Manifest,
    IReadOnlyList<LifecycleRelease> Releases,
    int VersionCount);

public enum LifecycleRisk
{
    Current,
    ApproachingEndOfLife,
    EndOfLife,
    Unknown,
}

public sealed class LifecycleCatalogLoader
{
    public async Task<LifecycleCatalog> LoadAsync(
        string directory,
        CancellationToken cancellationToken = default)
    {
        ReferencePackManifest manifest = await ReferenceManifestLoader.LoadAsync(directory, cancellationToken);
        List<LifecycleRelease> releases = [];
        int versionCount = 0;
        string releasesDirectory = Path.Combine(directory, "releases");
        foreach (string path in Directory.EnumerateFiles(releasesDirectory, "*.json").Order(StringComparer.Ordinal))
        {
            await using FileStream stream = File.OpenRead(path);
            using JsonDocument document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            string product = Path.GetFileNameWithoutExtension(path);
            JsonElement root = document.RootElement;
            Dictionary<string, (DateOnly? Date, string Name)> versions = [];
            if (root.TryGetProperty("versions", out JsonElement versionObject) &&
                versionObject.ValueKind == JsonValueKind.Object)
            {
                foreach (JsonProperty version in versionObject.EnumerateObject())
                {
                    string name = ReadString(version.Value, "name") ?? version.Name;
                    DateOnly? date = ParseDate(ReadString(version.Value, "date"));
                    versions[version.Name] = (date, name);
                }
            }

            versionCount += versions.Count;
            if (!root.TryGetProperty("releases", out JsonElement releaseObject) ||
                releaseObject.ValueKind != JsonValueKind.Object)
            {
                continue;
            }

            foreach (JsonProperty release in releaseObject.EnumerateObject())
            {
                string cycle = ReadString(release.Value, "name") ?? release.Name;
                DateOnly? releaseDate = ParseDate(ReadString(release.Value, "releaseDate"));
                DateOnly? endOfLife = ParseDate(ReadString(release.Value, "eol"));
                (string Latest, DateOnly? Date, int Count) latest = FindLatestVersion(versions, cycle);
                releases.Add(new LifecycleRelease(
                    product,
                    cycle,
                    releaseDate,
                    endOfLife,
                    latest.Date,
                    latest.Latest,
                    latest.Count));
            }
        }

        return new LifecycleCatalog(
            manifest,
            releases
                .OrderBy(static item => item.Product, StringComparer.OrdinalIgnoreCase)
                .ThenByDescending(static item => item.ReleaseDate)
                .ToArray(),
            versionCount);
    }

    private static (string Latest, DateOnly? Date, int Count) FindLatestVersion(
        IReadOnlyDictionary<string, (DateOnly? Date, string Name)> versions,
        string cycle)
    {
        KeyValuePair<string, (DateOnly? Date, string Name)>[] matches = versions
            .Where(item =>
                item.Key.Equals(cycle, StringComparison.OrdinalIgnoreCase) ||
                item.Key.StartsWith($"{cycle}.", StringComparison.OrdinalIgnoreCase) ||
                item.Key.StartsWith($"{cycle}-", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(static item => item.Value.Date)
            .ThenByDescending(static item => item.Key, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        return matches.Length == 0
            ? (string.Empty, null, 0)
            : (matches[0].Value.Name, matches[0].Value.Date, matches.Length);
    }

    private static string? ReadString(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out JsonElement value))
        {
            return null;
        }

        return value.ValueKind switch
        {
            JsonValueKind.String => value.GetString(),
            JsonValueKind.Number => value.GetRawText(),
            _ => null,
        };
    }

    private static DateOnly? ParseDate(string? value)
    {
        return DateOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly date)
            ? date
            : null;
    }
}

public sealed class LifecycleRiskAnalyzer
{
    public LifecycleRisk Classify(LifecycleRelease release, DateOnly asOf, int warningDays = 180)
    {
        if (release.EndOfLife is null)
        {
            return LifecycleRisk.Unknown;
        }

        if (release.EndOfLife < asOf)
        {
            return LifecycleRisk.EndOfLife;
        }

        return release.EndOfLife <= asOf.AddDays(warningDays)
            ? LifecycleRisk.ApproachingEndOfLife
            : LifecycleRisk.Current;
    }

    public IReadOnlyList<LifecycleRelease> CreateUpgradeCampaign(
        LifecycleCatalog catalog,
        DateOnly asOf,
        int horizonDays)
    {
        return catalog.Releases
            .Where(item =>
                item.EndOfLife is not null &&
                item.EndOfLife <= asOf.AddDays(horizonDays))
            .OrderBy(static item => item.EndOfLife)
            .ThenBy(static item => item.Product, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}

public static class LifecycleWorkbenchBuilder
{
    public static async Task<ReferenceWorkbenchSnapshot> BuildAsync(CancellationToken cancellationToken = default)
    {
        LifecycleCatalog catalog = await new LifecycleCatalogLoader().LoadAsync(
            ReferenceDataPaths.GetPackDirectory("lifecycle"),
            cancellationToken);
        LifecycleRiskAnalyzer analyzer = new();
        DateOnly asOf = DateOnly.FromDateTime(DateTime.UtcNow);
        List<ReferenceValidationIssue> issues = catalog.Releases
            .Where(static item =>
                item.ReleaseDate is not null &&
                item.EndOfLife is not null &&
                item.EndOfLife < item.ReleaseDate)
            .Select(static item => new ReferenceValidationIssue(
                "Error",
                "LIFECYCLE-DATE-ORDER",
                "End-of-life date precedes the release date.",
                $"{item.Product}:{item.Cycle}"))
            .ToList();
        IReadOnlyList<ReferenceDataRow> rows = catalog.Releases
            .Select(item =>
            {
                LifecycleRisk risk = analyzer.Classify(item, asOf);
                return new ReferenceDataRow(
                    $"{item.Product}:{item.Cycle}",
                    risk.ToString(),
                    $"{item.Product} {item.Cycle}",
                    $"Released {Format(item.ReleaseDate)} · EOL {Format(item.EndOfLife)} · Latest {item.LatestVersion} · {item.VersionCount:N0} versions",
                    $"{item.Product} {item.Cycle} {item.LatestVersion} {risk}");
            })
            .ToArray();
        IReadOnlyList<LifecycleRelease> campaign = analyzer.CreateUpgradeCampaign(catalog, asOf, 365);
        return new ReferenceWorkbenchSnapshot(
            "Software lifecycle intelligence",
            "Release cycles, end-of-life risk, latest versions, and upgrade campaign planning.",
            catalog.Manifest,
            rows,
            issues,
            new Dictionary<string, string>
            {
                ["Products"] = catalog.Releases.Select(static item => item.Product).Distinct().Count().ToString("N0"),
                ["Release cycles"] = catalog.Releases.Count.ToString("N0"),
                ["Detailed versions"] = catalog.VersionCount.ToString("N0"),
                ["EOL within one year"] = campaign.Count.ToString("N0"),
            });
    }

    private static string Format(DateOnly? value)
    {
        return value?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "unknown";
    }
}