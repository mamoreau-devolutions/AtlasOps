namespace AtlasOps.Features.Edge.EdgeTelemetryMonitoring;

public sealed record EdgeTelemetryMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);