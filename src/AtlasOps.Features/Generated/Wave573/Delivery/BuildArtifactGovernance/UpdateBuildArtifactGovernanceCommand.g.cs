namespace AtlasOps.Features.Delivery.BuildArtifactGovernance;

public sealed record UpdateBuildArtifactGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);