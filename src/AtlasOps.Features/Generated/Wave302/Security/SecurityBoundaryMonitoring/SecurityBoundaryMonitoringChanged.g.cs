namespace AtlasOps.Features.Security.SecurityBoundaryMonitoring;

public sealed record SecurityBoundaryMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);