namespace AtlasOps.Features.BusinessContinuity.RecoveryFailoverRecovery;

public sealed record RecoveryFailoverRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);