namespace AtlasOps.Features.Delivery.ReleasePipelineGovernance;

public sealed record UpdateReleasePipelineGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);