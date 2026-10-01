namespace AtlasOps.Features.Observability.ObservabilityDashboardMonitoring;

public sealed record ObservabilityDashboardMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);