namespace AtlasOps.Features.Security.SecurityBaselineMonitoring;

public sealed record SecurityBaselineMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);