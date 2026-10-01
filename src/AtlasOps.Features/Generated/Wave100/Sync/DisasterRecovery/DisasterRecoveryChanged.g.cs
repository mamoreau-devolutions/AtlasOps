namespace AtlasOps.Features.Sync.DisasterRecovery;

public sealed record DisasterRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);