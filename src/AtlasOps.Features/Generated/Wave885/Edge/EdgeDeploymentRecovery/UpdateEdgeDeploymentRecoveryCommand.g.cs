namespace AtlasOps.Features.Edge.EdgeDeploymentRecovery;

public sealed record UpdateEdgeDeploymentRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);