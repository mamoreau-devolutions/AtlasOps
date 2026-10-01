namespace AtlasOps.Features.Api.ApiTokenOptimization;

public sealed record ApiTokenOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);