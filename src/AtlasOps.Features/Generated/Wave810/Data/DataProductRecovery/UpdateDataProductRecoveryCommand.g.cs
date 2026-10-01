namespace AtlasOps.Features.Data.DataProductRecovery;

public sealed record UpdateDataProductRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);