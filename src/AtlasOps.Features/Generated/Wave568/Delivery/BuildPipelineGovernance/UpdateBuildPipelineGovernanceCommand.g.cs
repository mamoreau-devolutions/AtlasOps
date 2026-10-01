namespace AtlasOps.Features.Delivery.BuildPipelineGovernance;

public sealed record UpdateBuildPipelineGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);