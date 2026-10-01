namespace AtlasOps.Features.Compute.ComputeLifecycleGovernance;

public sealed record UpdateComputeLifecycleGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);