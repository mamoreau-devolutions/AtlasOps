namespace AtlasOps.Features.BusinessContinuity.RecoveryFailoverOptimization;

public sealed record RecoveryFailoverOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);