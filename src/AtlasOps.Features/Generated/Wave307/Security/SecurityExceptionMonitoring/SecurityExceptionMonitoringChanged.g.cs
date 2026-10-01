namespace AtlasOps.Features.Security.SecurityExceptionMonitoring;

public sealed record SecurityExceptionMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);