namespace AtlasOps.Features.Edge.EdgeDeploymentGovernance;

public sealed record UpdateEdgeDeploymentGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);