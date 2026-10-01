namespace AtlasOps.Features.Delivery.ReleaseRollbackRecovery;

public sealed record ReleaseRollbackRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);