namespace AtlasOps.Features.Delivery.ReleaseEnvironmentMonitoring;

public sealed record ReleaseEnvironmentMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);