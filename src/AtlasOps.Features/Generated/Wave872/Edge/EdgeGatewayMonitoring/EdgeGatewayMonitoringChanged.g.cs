namespace AtlasOps.Features.Edge.EdgeGatewayMonitoring;

public sealed record EdgeGatewayMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);