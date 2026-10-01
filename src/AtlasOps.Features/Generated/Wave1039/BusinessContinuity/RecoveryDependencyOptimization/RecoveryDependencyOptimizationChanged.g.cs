namespace AtlasOps.Features.BusinessContinuity.RecoveryDependencyOptimization;

public sealed record RecoveryDependencyOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);