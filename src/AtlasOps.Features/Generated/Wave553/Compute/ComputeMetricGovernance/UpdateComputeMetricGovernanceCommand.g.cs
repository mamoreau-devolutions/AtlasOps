namespace AtlasOps.Features.Compute.ComputeMetricGovernance;

public sealed record UpdateComputeMetricGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);