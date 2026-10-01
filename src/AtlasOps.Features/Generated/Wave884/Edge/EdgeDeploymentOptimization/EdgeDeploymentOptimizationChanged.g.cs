namespace AtlasOps.Features.Edge.EdgeDeploymentOptimization;

public sealed record EdgeDeploymentOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);