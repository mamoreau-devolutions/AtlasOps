namespace AtlasOps.Features.Compute.ComputeImageRecovery;

public sealed record UpdateComputeImageRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);