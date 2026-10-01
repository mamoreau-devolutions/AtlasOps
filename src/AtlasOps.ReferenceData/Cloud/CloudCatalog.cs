namespace AtlasOps.ReferenceData.Cloud;

using System.Globalization;
using System.Text.Json;

using AtlasOps.ReferenceData.Infrastructure;

public sealed record CloudOffer(
    string Provider,
    string Sku,
    string Name,
    string Family,
    string Region,
    double VCpu,
    double MemoryGiB,
    double GpuCount,
    string Architecture,
    string Storage,
    decimal HourlyUsd,
    decimal MonthlyUsd);

public sealed record CloudCatalog(
    ReferencePackManifest Manifest,
    IReadOnlyList<CloudOffer> Offers);

public sealed class CloudCatalogLoader
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public async Task<CloudCatalog> LoadAsync(
        string directory,
        CancellationToken cancellationToken = default)
    {
        ReferencePackManifest manifest = await ReferenceManifestLoader.LoadAsync(directory, cancellationToken);
        List<CloudOffer> offers = [];
        using StreamReader reader = new(Path.Combine(directory, "offers.ndjson"));
        while (await reader.ReadLineAsync(cancellationToken) is { } line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            CloudOffer? offer = JsonSerializer.Deserialize<CloudOffer>(line, Options);
            if (offer is not null)
            {
                offers.Add(offer);
            }
        }

        return new CloudCatalog(manifest, offers);
    }
}

public sealed class CloudCostAnalyzer
{
    public decimal CalculateMonthlyCost(CloudOffer offer, decimal hours = 730m)
    {
        return decimal.Round(offer.HourlyUsd * hours, 2, MidpointRounding.AwayFromZero);
    }

    public decimal PricePerVCpu(CloudOffer offer)
    {
        return offer.VCpu <= 0
            ? decimal.MaxValue
            : decimal.Round(offer.HourlyUsd / (decimal)offer.VCpu, 6);
    }

    public decimal PricePerGiB(CloudOffer offer)
    {
        return offer.MemoryGiB <= 0
            ? decimal.MaxValue
            : decimal.Round(offer.HourlyUsd / (decimal)offer.MemoryGiB, 6);
    }

    public IReadOnlyList<CloudOffer> Rank(
        CloudCatalog catalog,
        double minimumVCpu,
        double minimumMemoryGiB,
        string? architecture,
        int limit = 100)
    {
        return catalog.Offers
            .Where(item =>
                item.VCpu >= minimumVCpu &&
                item.MemoryGiB >= minimumMemoryGiB &&
                (string.IsNullOrWhiteSpace(architecture) ||
                 item.Architecture.Contains(architecture, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(static item => item.HourlyUsd)
            .ThenBy(static item => item.Provider, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static item => item.Sku, StringComparer.OrdinalIgnoreCase)
            .Take(limit)
            .ToArray();
    }
}

public static class CloudWorkbenchBuilder
{
    public static async Task<ReferenceWorkbenchSnapshot> BuildAsync(CancellationToken cancellationToken = default)
    {
        CloudCatalog catalog = await new CloudCatalogLoader().LoadAsync(
            ReferenceDataPaths.GetPackDirectory("cloud"),
            cancellationToken);
        CloudCostAnalyzer analyzer = new();
        List<ReferenceValidationIssue> issues = catalog.Offers
            .Where(static item =>
                item.VCpu <= 0 ||
                item.MemoryGiB <= 0 ||
                item.HourlyUsd <= 0 ||
                item.MonthlyUsd <= 0)
            .Take(1_000)
            .Select(static item => new ReferenceValidationIssue(
                "Error",
                "CLOUD-OFFER-INVALID",
                "Cloud offer contains non-positive capacity or pricing.",
                $"{item.Provider}:{item.Sku}:{item.Region}"))
            .ToList();
        IReadOnlyList<ReferenceDataRow> rows = catalog.Offers
            .Select(item => new ReferenceDataRow(
                $"{item.Provider}:{item.Sku}:{item.Region}",
                item.Provider,
                $"{item.Provider} {item.Sku}",
                $"{item.Region} · {item.VCpu:N0} vCPU · {item.MemoryGiB:N1} GiB · {item.Architecture} · ${item.HourlyUsd:N4}/h · ${analyzer.CalculateMonthlyCost(item):N2}/mo",
                $"{item.Provider} {item.Sku} {item.Name} {item.Family} {item.Region} {item.Architecture} {item.Storage}"))
            .ToArray();
        IReadOnlyList<CloudOffer> baseline = analyzer.Rank(catalog, 4, 16, null);
        return new ReferenceWorkbenchSnapshot(
            "Multi-cloud instance economics",
            "Normalized AWS, Azure, and GCP compute offers with capacity filters and cost rankings.",
            catalog.Manifest,
            rows,
            issues,
            new Dictionary<string, string>
            {
                ["Regional offers"] = catalog.Offers.Count.ToString("N0", CultureInfo.InvariantCulture),
                ["Distinct SKUs"] = catalog.Offers.Select(static item => $"{item.Provider}:{item.Sku}").Distinct().Count().ToString("N0", CultureInfo.InvariantCulture),
                ["Regions"] = catalog.Offers.Select(static item => $"{item.Provider}:{item.Region}").Distinct().Count().ToString("N0", CultureInfo.InvariantCulture),
                ["Cheapest 4 vCPU / 16 GiB"] = baseline.FirstOrDefault() is { } cheapest
                    ? $"{cheapest.Provider} {cheapest.Sku} ${cheapest.HourlyUsd:N4}/h"
                    : "none",
            });
    }
}