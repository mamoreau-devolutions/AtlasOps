namespace AtlasOps.Features.Security.SecuritySessionMonitoring;

public sealed record SecuritySessionMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);