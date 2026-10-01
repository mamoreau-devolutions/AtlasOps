namespace AtlasOps.ReferenceData.Localization;

using System.Globalization;
using System.Text.Json;

using AtlasOps.ReferenceData.Infrastructure;

public sealed record LocaleSummary(
    string Locale,
    int LanguageNames,
    int TerritoryNames,
    int CurrencyNames,
    int TimeZoneNames);

public sealed record TimeZoneSummary(
    string Id,
    IReadOnlyList<string> Countries,
    string Coordinates,
    string Comment);

public sealed record WindowsZoneMapping(
    string WindowsId,
    string Territory,
    IReadOnlyList<string> IanaIds);

public sealed record LocalizationCatalog(
    ReferencePackManifest Manifest,
    IReadOnlyList<LocaleSummary> Locales,
    IReadOnlyList<TimeZoneSummary> TimeZones,
    IReadOnlyList<WindowsZoneMapping> WindowsMappings);

public sealed class LocalizationCatalogLoader
{
    public async Task<LocalizationCatalog> LoadAsync(
        string directory,
        CancellationToken cancellationToken = default)
    {
        ReferencePackManifest manifest = await ReferenceManifestLoader.LoadAsync(directory, cancellationToken);
        IReadOnlyList<LocaleSummary> locales = await this.LoadLocalesAsync(directory, cancellationToken);
        IReadOnlyList<TimeZoneSummary> zones = await this.LoadTimeZonesAsync(directory, cancellationToken);
        IReadOnlyList<WindowsZoneMapping> mappings = await this.LoadWindowsMappingsAsync(directory, cancellationToken);
        return new LocalizationCatalog(manifest, locales, zones, mappings);
    }

    private async Task<IReadOnlyList<LocaleSummary>> LoadLocalesAsync(
        string directory,
        CancellationToken cancellationToken)
    {
        string localNamesRoot = Path.Combine(
            directory,
            "cldr",
            "cldr-localenames-full",
            "package",
            "main");
        string numbersRoot = Path.Combine(directory, "cldr", "cldr-numbers-full", "package", "main");
        string datesRoot = Path.Combine(directory, "cldr", "cldr-dates-full", "package", "main");
        List<LocaleSummary> result = [];
        foreach (string localeDirectory in Directory.EnumerateDirectories(localNamesRoot).Order(StringComparer.Ordinal))
        {
            cancellationToken.ThrowIfCancellationRequested();
            string locale = Path.GetFileName(localeDirectory);
            int languageCount = await CountPropertiesAsync(
                Path.Combine(localeDirectory, "languages.json"),
                ["main", locale, "localeDisplayNames", "languages"],
                cancellationToken);
            int territoryCount = await CountPropertiesAsync(
                Path.Combine(localeDirectory, "territories.json"),
                ["main", locale, "localeDisplayNames", "territories"],
                cancellationToken);
            int currencyCount = await CountPropertiesAsync(
                Path.Combine(numbersRoot, locale, "currencies.json"),
                ["main", locale, "numbers", "currencies"],
                cancellationToken);
            int timeZoneCount = await CountPropertiesAsync(
                Path.Combine(datesRoot, locale, "timeZoneNames.json"),
                ["main", locale, "dates", "timeZoneNames", "zone"],
                cancellationToken);
            result.Add(new LocaleSummary(locale, languageCount, territoryCount, currencyCount, timeZoneCount));
        }

        return result;
    }

    private async Task<IReadOnlyList<TimeZoneSummary>> LoadTimeZonesAsync(
        string directory,
        CancellationToken cancellationToken)
    {
        string path = Path.Combine(directory, "tzdb", "zone1970.tab");
        string[] lines = await File.ReadAllLinesAsync(path, cancellationToken);
        return lines
            .Where(static line => line.Length > 0 && line[0] != '#')
            .Select(static line => line.Split('\t'))
            .Where(static fields => fields.Length >= 3)
            .Select(static fields => new TimeZoneSummary(
                fields[2],
                fields[0].Split(',', StringSplitOptions.RemoveEmptyEntries),
                fields[1],
                fields.Length > 3 ? fields[3] : string.Empty))
            .OrderBy(static zone => zone.Id, StringComparer.Ordinal)
            .ToArray();
    }

    private async Task<IReadOnlyList<WindowsZoneMapping>> LoadWindowsMappingsAsync(
        string directory,
        CancellationToken cancellationToken)
    {
        string path = Path.Combine(
            directory,
            "cldr",
            "cldr-core",
            "package",
            "supplemental",
            "windowsZones.json");
        await using FileStream stream = File.OpenRead(path);
        using JsonDocument document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        JsonElement mapTimezones = document.RootElement
            .GetProperty("supplemental")
            .GetProperty("windowsZones")
            .GetProperty("mapTimezones");
        List<WindowsZoneMapping> result = [];
        foreach (JsonElement item in mapTimezones.EnumerateArray())
        {
            JsonElement mapZone = item.GetProperty("mapZone");
            result.Add(new WindowsZoneMapping(
                mapZone.GetProperty("_other").GetString() ?? string.Empty,
                mapZone.GetProperty("_territory").GetString() ?? string.Empty,
                (mapZone.GetProperty("_type").GetString() ?? string.Empty)
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)));
        }

        return result;
    }

    private static async Task<int> CountPropertiesAsync(
        string path,
        IReadOnlyList<string> propertyPath,
        CancellationToken cancellationToken)
    {
        if (!File.Exists(path))
        {
            return 0;
        }

        await using FileStream stream = File.OpenRead(path);
        using JsonDocument document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        JsonElement element = document.RootElement;
        foreach (string property in propertyPath)
        {
            if (!element.TryGetProperty(property, out element))
            {
                return 0;
            }
        }

        return element.ValueKind == JsonValueKind.Object
            ? element.EnumerateObject().Count()
            : 0;
    }
}

public sealed class LocaleFallbackResolver
{
    public IReadOnlyList<string> Resolve(string locale, IReadOnlySet<string> availableLocales)
    {
        List<string> result = [];
        string candidate = locale.Replace('_', '-');
        while (!string.IsNullOrWhiteSpace(candidate))
        {
            if (availableLocales.Contains(candidate))
            {
                result.Add(candidate);
            }

            int separator = candidate.LastIndexOf('-');
            candidate = separator < 0 ? string.Empty : candidate[..separator];
        }

        if (availableLocales.Contains("root"))
        {
            result.Add("root");
        }

        return result;
    }
}

public sealed class TimeZoneAnalysisService
{
    public DateTimeOffset Convert(DateTimeOffset instant, string timeZoneId)
    {
        TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        return TimeZoneInfo.ConvertTime(instant, zone);
    }

    public bool IsAmbiguous(DateTime localTime, string timeZoneId)
    {
        return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId).IsAmbiguousTime(localTime);
    }

    public bool IsInvalid(DateTime localTime, string timeZoneId)
    {
        return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId).IsInvalidTime(localTime);
    }
}

public static class LocalizationWorkbenchBuilder
{
    public static async Task<ReferenceWorkbenchSnapshot> BuildAsync(CancellationToken cancellationToken = default)
    {
        LocalizationCatalog catalog = await new LocalizationCatalogLoader().LoadAsync(
            ReferenceDataPaths.GetPackDirectory("localization"),
            cancellationToken);
        List<ReferenceDataRow> rows = [];
        rows.AddRange(catalog.Locales.Select(static item => new ReferenceDataRow(
            $"locale:{item.Locale}",
            "Locale",
            item.Locale,
            $"{item.LanguageNames:N0} languages · {item.TerritoryNames:N0} territories · {item.CurrencyNames:N0} currencies · {item.TimeZoneNames:N0} zones",
            $"{item.Locale} locale languages territories currencies time zones")));
        rows.AddRange(catalog.TimeZones.Select(static item => new ReferenceDataRow(
            $"zone:{item.Id}",
            "IANA time zone",
            item.Id,
            $"{string.Join(", ", item.Countries)} · {item.Coordinates} · {item.Comment}",
            $"{item.Id} {string.Join(' ', item.Countries)} {item.Comment} {item.Coordinates}")));
        rows.AddRange(catalog.WindowsMappings.Select(static item => new ReferenceDataRow(
            $"windows-zone:{item.WindowsId}:{item.Territory}",
            "Windows / IANA mapping",
            item.WindowsId,
            $"{item.Territory} · {string.Join(", ", item.IanaIds)}",
            $"{item.WindowsId} {item.Territory} {string.Join(' ', item.IanaIds)}")));
        List<ReferenceValidationIssue> issues = catalog.Locales
            .Where(static item => item.LanguageNames == 0 || item.TerritoryNames == 0)
            .Select(static item => new ReferenceValidationIssue(
                "Warning",
                "CLDR-LOCALE-INCOMPLETE",
                "Locale does not contain both language and territory display-name tables.",
                item.Locale))
            .ToList();
        return new ReferenceWorkbenchSnapshot(
            "Locale and time-zone intelligence",
            "CLDR locale coverage, display-name inventories, IANA zones, and Windows mappings.",
            catalog.Manifest,
            rows,
            issues,
            new Dictionary<string, string>
            {
                ["CLDR locales"] = catalog.Locales.Count.ToString("N0", CultureInfo.InvariantCulture),
                ["IANA zones"] = catalog.TimeZones.Count.ToString("N0", CultureInfo.InvariantCulture),
                ["Windows mappings"] = catalog.WindowsMappings.Count.ToString("N0", CultureInfo.InvariantCulture),
                ["Localized names"] = catalog.Locales.Sum(static item =>
                    item.LanguageNames + item.TerritoryNames + item.CurrencyNames + item.TimeZoneNames).ToString("N0", CultureInfo.InvariantCulture),
            });
    }
}