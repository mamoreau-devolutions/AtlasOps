namespace AtlasOps.Features.Data.DataRetentionRecovery;

public sealed record UpdateDataRetentionRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);