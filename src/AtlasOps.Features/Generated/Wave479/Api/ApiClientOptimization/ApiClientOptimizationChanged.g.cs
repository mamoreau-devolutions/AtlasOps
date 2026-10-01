namespace AtlasOps.Features.Api.ApiClientOptimization;

public sealed record ApiClientOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);