namespace AtlasOps.Features.Compute.ComputePatchGovernance;

public sealed record UpdateComputePatchGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);