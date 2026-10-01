namespace AtlasOps.Features.Api.ApiVersionOptimization;

public sealed record ApiVersionOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);