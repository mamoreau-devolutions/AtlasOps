namespace AtlasOps.Features.Security.SecurityKeyMonitoring;

public sealed record SecurityKeyMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);