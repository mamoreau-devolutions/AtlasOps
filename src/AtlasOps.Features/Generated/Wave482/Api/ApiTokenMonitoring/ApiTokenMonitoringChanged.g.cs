namespace AtlasOps.Features.Api.ApiTokenMonitoring;

public sealed record ApiTokenMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);