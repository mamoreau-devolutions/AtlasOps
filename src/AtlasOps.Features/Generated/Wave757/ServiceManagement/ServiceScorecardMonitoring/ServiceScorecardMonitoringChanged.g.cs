namespace AtlasOps.Features.ServiceManagement.ServiceScorecardMonitoring;

public sealed record ServiceScorecardMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);