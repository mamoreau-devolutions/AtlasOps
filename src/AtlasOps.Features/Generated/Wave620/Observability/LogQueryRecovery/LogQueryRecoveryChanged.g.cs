namespace AtlasOps.Features.Observability.LogQueryRecovery;

public sealed record LogQueryRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);