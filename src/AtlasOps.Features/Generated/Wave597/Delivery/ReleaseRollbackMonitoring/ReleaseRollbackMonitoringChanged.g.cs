namespace AtlasOps.Features.Delivery.ReleaseRollbackMonitoring;

public sealed record ReleaseRollbackMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);