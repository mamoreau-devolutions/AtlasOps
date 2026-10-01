namespace AtlasOps.Features.Api.ApiEndpointOptimization;

public sealed record ApiEndpointOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);