namespace AtlasOps.Features.Api.ApiDeploymentOptimization;

public sealed record ApiDeploymentOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);