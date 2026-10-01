namespace AtlasOps.Features.Compute.ComputeScheduleGovernance;

public sealed record UpdateComputeScheduleGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);