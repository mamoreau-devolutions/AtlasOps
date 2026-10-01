namespace AtlasOps.Features.Security.SecurityExceptionOptimization;

public sealed record SecurityExceptionOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);