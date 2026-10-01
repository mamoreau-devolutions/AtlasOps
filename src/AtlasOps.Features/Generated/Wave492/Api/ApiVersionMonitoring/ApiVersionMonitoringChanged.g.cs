namespace AtlasOps.Features.Api.ApiVersionMonitoring;

public sealed record ApiVersionMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);