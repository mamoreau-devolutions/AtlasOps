namespace AtlasOps.Features.Network.NetworkDnsZoneMonitoring;

public sealed record NetworkDnsZoneMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);