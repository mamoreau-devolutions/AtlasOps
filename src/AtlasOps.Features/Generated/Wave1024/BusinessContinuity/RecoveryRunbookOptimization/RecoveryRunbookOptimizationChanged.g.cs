namespace AtlasOps.Features.BusinessContinuity.RecoveryRunbookOptimization;

public sealed record RecoveryRunbookOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);