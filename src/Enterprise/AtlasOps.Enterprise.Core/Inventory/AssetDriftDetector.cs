namespace AtlasOps.Enterprise.Core.Inventory;

using AtlasOps.Enterprise.Contracts.Inventory;

public sealed class AssetDriftDetector
{
    private static readonly IReadOnlySet<string> CriticalProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "encryption",
        "firewall",
        "owner",
        "environment",
        "operatingSystem",
    };

    public IReadOnlyList<AssetDrift> Detect(AssetRecord asset, DesiredAssetState desired)
    {
        HashSet<string> properties = new(desired.Properties.Keys, StringComparer.OrdinalIgnoreCase);
        properties.UnionWith(asset.Properties.Keys);
        List<AssetDrift> drift = [];

        foreach (string property in properties.Order(StringComparer.OrdinalIgnoreCase))
        {
            bool hasExpected = desired.Properties.TryGetValue(property, out string? expected);
            bool hasActual = asset.Properties.TryGetValue(property, out string? actual);
            DriftKind kind = (hasExpected, hasActual) switch
            {
                (true, false) => DriftKind.Missing,
                (false, true) => DriftKind.Unexpected,
                (true, true) when !string.Equals(expected, actual, StringComparison.OrdinalIgnoreCase) => DriftKind.Changed,
                _ => DriftKind.Compliant,
            };
            DriftSeverity severity = CalculateSeverity(property, kind);
            string explanation = kind switch
            {
                DriftKind.Missing => $"Expected property '{property}' is missing.",
                DriftKind.Unexpected => $"Property '{property}' is present but not declared in desired state.",
                DriftKind.Changed => $"Property '{property}' differs from desired state.",
                _ => $"Property '{property}' matches desired state.",
            };
            drift.Add(new(property, expected, actual, kind, severity, explanation));
        }

        return drift;
    }

    private static DriftSeverity CalculateSeverity(string property, DriftKind kind)
    {
        if (kind == DriftKind.Compliant)
        {
            return DriftSeverity.Information;
        }

        if (CriticalProperties.Contains(property))
        {
            return kind == DriftKind.Missing ? DriftSeverity.Critical : DriftSeverity.High;
        }

        return kind == DriftKind.Unexpected ? DriftSeverity.Low : DriftSeverity.Medium;
    }
}
