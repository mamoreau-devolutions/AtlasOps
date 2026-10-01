namespace AtlasOps.Features.Api.ApiGatewayMonitoring;

public sealed record ApiGatewayMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);