namespace AtlasOps.Features.Observability.ObservabilitySloRecovery;

public sealed record ObservabilitySloRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);