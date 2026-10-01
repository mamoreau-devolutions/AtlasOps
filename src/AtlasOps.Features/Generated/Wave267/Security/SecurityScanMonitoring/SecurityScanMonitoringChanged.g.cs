namespace AtlasOps.Features.Security.SecurityScanMonitoring;

public sealed record SecurityScanMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);