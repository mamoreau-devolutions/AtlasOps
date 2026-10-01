namespace AtlasOps.Features.Delivery.ReleaseMetricMonitoring;

public sealed record ReleaseMetricMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);