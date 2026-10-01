namespace AtlasOps.Features.Observability.ObservabilityExportRecovery;

public sealed record ObservabilityExportRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);