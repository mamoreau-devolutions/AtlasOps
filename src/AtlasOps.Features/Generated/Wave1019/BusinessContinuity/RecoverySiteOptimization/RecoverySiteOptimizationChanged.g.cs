namespace AtlasOps.Features.BusinessContinuity.RecoverySiteOptimization;

public sealed record RecoverySiteOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);