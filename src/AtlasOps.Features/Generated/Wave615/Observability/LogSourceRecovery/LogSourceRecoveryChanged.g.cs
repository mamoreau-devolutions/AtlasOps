namespace AtlasOps.Features.Observability.LogSourceRecovery;

public sealed record LogSourceRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);