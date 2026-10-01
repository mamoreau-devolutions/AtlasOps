namespace AtlasOps.Features.Security.SecurityFindingMonitoring;

public sealed record SecurityFindingMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);