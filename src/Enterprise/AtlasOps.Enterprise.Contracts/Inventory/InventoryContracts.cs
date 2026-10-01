namespace AtlasOps.Enterprise.Contracts.Inventory;

public enum AssetLifecycleState
{
    Discovered,
    Active,
    Maintenance,
    Retired,
    Missing,
}

public enum AssetEvidenceSource
{
    Agent,
    CloudApi,
    Directory,
    NetworkScan,
    Manual,
    Import,
}

public enum DriftKind
{
    Missing,
    Unexpected,
    Changed,
    Compliant,
}

public enum DriftSeverity
{
    Information,
    Low,
    Medium,
    High,
    Critical,
}

public sealed record AssetIdentifier(
    string Scheme,
    string Value,
    bool IsImmutable);

public sealed record AssetRecord(
    string Id,
    string DisplayName,
    string AssetType,
    string Owner,
    AssetLifecycleState LifecycleState,
    IReadOnlyList<AssetIdentifier> Identifiers,
    IReadOnlyDictionary<string, string> Properties,
    DateTimeOffset FirstSeenAt,
    DateTimeOffset LastSeenAt);

public sealed record AssetEvidence(
    string Id,
    AssetEvidenceSource Source,
    string SourceInstance,
    string DisplayName,
    string AssetType,
    IReadOnlyList<AssetIdentifier> Identifiers,
    IReadOnlyDictionary<string, string> Properties,
    DateTimeOffset ObservedAt,
    double SourceReliability);

public sealed record AssetMatchReason(
    string Kind,
    string Description,
    double Weight,
    bool Matched);

public sealed record AssetMatchResult(
    string AssetId,
    string EvidenceId,
    double Score,
    bool IsConfidentMatch,
    IReadOnlyList<AssetMatchReason> Reasons);

public sealed record ReconciledProperty(
    string Name,
    string Value,
    AssetEvidenceSource Source,
    string EvidenceId,
    DateTimeOffset ObservedAt,
    double Confidence);

public sealed record AssetReconciliationResult(
    AssetRecord Asset,
    IReadOnlyList<ReconciledProperty> Properties,
    IReadOnlyList<string> Diagnostics);

public sealed record DesiredAssetState(
    string AssetId,
    IReadOnlyDictionary<string, string> Properties);

public sealed record AssetDrift(
    string Property,
    string? ExpectedValue,
    string? ActualValue,
    DriftKind Kind,
    DriftSeverity Severity,
    string Explanation);

public sealed record AssetRelationship(
    string SourceAssetId,
    string TargetAssetId,
    string Kind,
    bool IsCritical);

public sealed record AssetTopologyNode(
    AssetRecord Asset,
    int Distance,
    IReadOnlyList<string> Path);

public sealed record InventoryQualityReport(
    int TotalAssets,
    int StaleAssets,
    int IncompleteAssets,
    double CompletenessPercent,
    IReadOnlyList<string> Diagnostics);
