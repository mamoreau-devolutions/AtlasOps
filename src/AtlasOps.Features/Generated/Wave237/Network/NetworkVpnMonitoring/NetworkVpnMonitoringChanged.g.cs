namespace AtlasOps.Features.Network.NetworkVpnMonitoring;

public sealed record NetworkVpnMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);