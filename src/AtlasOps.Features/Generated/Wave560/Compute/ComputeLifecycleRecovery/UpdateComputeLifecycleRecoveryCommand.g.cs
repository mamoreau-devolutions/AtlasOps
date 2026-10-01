namespace AtlasOps.Features.Compute.ComputeLifecycleRecovery;

public sealed record UpdateComputeLifecycleRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);