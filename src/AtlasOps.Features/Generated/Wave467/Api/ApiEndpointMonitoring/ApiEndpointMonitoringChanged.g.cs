namespace AtlasOps.Features.Api.ApiEndpointMonitoring;

public sealed record ApiEndpointMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);