namespace AtlasOps.Features.Edge.EdgeNetworkMonitoring;

public sealed record EdgeNetworkMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);