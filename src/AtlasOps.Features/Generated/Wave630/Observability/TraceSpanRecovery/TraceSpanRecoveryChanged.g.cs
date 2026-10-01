namespace AtlasOps.Features.Observability.TraceSpanRecovery;

public sealed record TraceSpanRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);