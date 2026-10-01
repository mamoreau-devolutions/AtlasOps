namespace AtlasOps.Features.Edge.EdgeGatewayOptimization;

public sealed record EdgeGatewayOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);