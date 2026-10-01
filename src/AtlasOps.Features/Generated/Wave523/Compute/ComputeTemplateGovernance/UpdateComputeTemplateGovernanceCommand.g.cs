namespace AtlasOps.Features.Compute.ComputeTemplateGovernance;

public sealed record UpdateComputeTemplateGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);