namespace AtlasOps.Features.Api.ApiGatewayOptimization;

public sealed record ApiGatewayOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);