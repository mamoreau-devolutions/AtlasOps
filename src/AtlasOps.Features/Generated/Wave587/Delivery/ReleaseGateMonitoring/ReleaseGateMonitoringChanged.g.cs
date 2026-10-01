namespace AtlasOps.Features.Delivery.ReleaseGateMonitoring;

public sealed record ReleaseGateMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);