namespace AtlasOps.Features.Compute.ComputeImageGovernance;

public sealed record UpdateComputeImageGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);