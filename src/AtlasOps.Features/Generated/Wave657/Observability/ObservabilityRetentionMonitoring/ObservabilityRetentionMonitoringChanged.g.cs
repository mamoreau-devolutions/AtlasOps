namespace AtlasOps.Features.Observability.ObservabilityRetentionMonitoring;

public sealed record ObservabilityRetentionMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);