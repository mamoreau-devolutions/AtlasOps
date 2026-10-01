namespace AtlasOps.Features.Compute.ComputePatchRecovery;

public sealed record UpdateComputePatchRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);