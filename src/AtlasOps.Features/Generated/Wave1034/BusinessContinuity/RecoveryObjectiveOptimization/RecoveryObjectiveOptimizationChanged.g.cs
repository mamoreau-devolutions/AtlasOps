namespace AtlasOps.Features.BusinessContinuity.RecoveryObjectiveOptimization;

public sealed record RecoveryObjectiveOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);