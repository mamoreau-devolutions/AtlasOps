namespace AtlasOps.Features.Api.ApiHealthMonitoring;

public sealed record ApiHealthMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);