namespace AtlasOps.Features.Network.NetworkRouteMonitoring;

public sealed record NetworkRouteMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);