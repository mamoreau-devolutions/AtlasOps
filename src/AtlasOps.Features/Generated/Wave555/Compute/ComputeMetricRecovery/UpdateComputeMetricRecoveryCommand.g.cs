namespace AtlasOps.Features.Compute.ComputeMetricRecovery;

public sealed record UpdateComputeMetricRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);