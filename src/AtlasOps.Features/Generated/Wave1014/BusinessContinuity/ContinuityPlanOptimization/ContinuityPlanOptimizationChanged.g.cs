namespace AtlasOps.Features.BusinessContinuity.ContinuityPlanOptimization;

public sealed record ContinuityPlanOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);