namespace AtlasOps.Features.Network.NetworkPeerMonitoring;

public sealed record NetworkPeerMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);