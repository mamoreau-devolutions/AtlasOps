namespace AtlasOps.Features.Compute.ComputeScaleSetRecovery;

public sealed record UpdateComputeScaleSetRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);