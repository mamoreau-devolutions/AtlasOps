namespace AtlasOps.Features.Cloud.GcpProjectMonitoring;

public sealed record GcpProjectMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);