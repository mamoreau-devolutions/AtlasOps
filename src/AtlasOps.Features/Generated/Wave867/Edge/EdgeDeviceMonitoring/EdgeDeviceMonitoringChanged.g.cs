namespace AtlasOps.Features.Edge.EdgeDeviceMonitoring;

public sealed record EdgeDeviceMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);