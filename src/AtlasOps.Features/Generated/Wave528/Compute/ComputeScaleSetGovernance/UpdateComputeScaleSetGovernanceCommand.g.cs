namespace AtlasOps.Features.Compute.ComputeScaleSetGovernance;

public sealed record UpdateComputeScaleSetGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);