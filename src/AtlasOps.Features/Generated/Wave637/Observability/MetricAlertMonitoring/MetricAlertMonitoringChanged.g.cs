namespace AtlasOps.Features.Observability.MetricAlertMonitoring;

public sealed record MetricAlertMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);