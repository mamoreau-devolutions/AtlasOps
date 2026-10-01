namespace AtlasOps.Enterprise.Core.Inventory;

using AtlasOps.Enterprise.Contracts.Inventory;

public sealed class InventoryQualityService
{
    private static readonly string[] RequiredProperties = ["environment", "location", "operatingSystem"];

    public InventoryQualityReport Analyze(IReadOnlyList<AssetRecord> assets, DateTimeOffset now, TimeSpan staleAfter)
    {
        int stale = 0;
        int incomplete = 0;
        int expectedFields = assets.Count * RequiredProperties.Length;
        int populatedFields = 0;
        List<string> diagnostics = [];

        foreach (AssetRecord asset in assets)
        {
            if (now - asset.LastSeenAt > staleAfter)
            {
                stale++;
                diagnostics.Add($"Asset '{asset.DisplayName}' has not been observed since {asset.LastSeenAt:O}.");
            }

            string[] missing = RequiredProperties
                .Where(property => !asset.Properties.TryGetValue(property, out string? value) || string.IsNullOrWhiteSpace(value))
                .ToArray();
            populatedFields += RequiredProperties.Length - missing.Length;
            if (missing.Length > 0)
            {
                incomplete++;
                diagnostics.Add($"Asset '{asset.DisplayName}' is missing: {string.Join(", ", missing)}.");
            }
        }

        double completeness = expectedFields == 0 ? 100d : (double)populatedFields / expectedFields * 100d;
        return new(assets.Count, stale, incomplete, Math.Round(completeness, 2), diagnostics);
    }
}
