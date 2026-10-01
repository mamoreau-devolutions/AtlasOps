namespace AtlasOps.Features.BusinessContinuity.RecoveryDependencyRecovery;

public sealed record RecoveryDependencyRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);