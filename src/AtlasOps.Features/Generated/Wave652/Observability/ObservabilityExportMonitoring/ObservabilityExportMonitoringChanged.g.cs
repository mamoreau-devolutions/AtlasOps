namespace AtlasOps.Features.Observability.ObservabilityExportMonitoring;

public sealed record ObservabilityExportMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);