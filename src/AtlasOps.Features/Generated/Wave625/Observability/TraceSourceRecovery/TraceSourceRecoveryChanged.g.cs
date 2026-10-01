namespace AtlasOps.Features.Observability.TraceSourceRecovery;

public sealed record TraceSourceRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);