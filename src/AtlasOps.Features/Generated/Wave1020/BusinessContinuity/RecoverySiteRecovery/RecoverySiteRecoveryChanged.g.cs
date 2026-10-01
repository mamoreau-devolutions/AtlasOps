namespace AtlasOps.Features.BusinessContinuity.RecoverySiteRecovery;

public sealed record RecoverySiteRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);