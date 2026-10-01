namespace AtlasOps.Features.Data.DataQualityRecovery;

public sealed record UpdateDataQualityRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);