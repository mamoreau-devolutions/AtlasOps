namespace AtlasOps.Features.Compute.ComputeConsoleGovernance;

public sealed record UpdateComputeConsoleGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);