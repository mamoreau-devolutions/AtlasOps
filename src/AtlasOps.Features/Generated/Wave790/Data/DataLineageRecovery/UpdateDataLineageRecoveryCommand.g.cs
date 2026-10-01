namespace AtlasOps.Features.Data.DataLineageRecovery;

public sealed record UpdateDataLineageRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);