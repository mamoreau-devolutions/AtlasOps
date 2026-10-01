namespace AtlasOps.Features.BusinessContinuity.RecoveryRunbookRecovery;

public sealed record RecoveryRunbookRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);