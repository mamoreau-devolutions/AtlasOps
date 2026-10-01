namespace AtlasOps.Features.Cloud.CloudNetworkMonitoring;

public sealed record CloudNetworkMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);