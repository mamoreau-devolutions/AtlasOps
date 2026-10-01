namespace AtlasOps.Features.Data.DataDatasetRecovery;

public sealed record UpdateDataDatasetRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);