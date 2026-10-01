namespace AtlasOps.Features.Network.NetworkAddressMonitoring;

public sealed record NetworkAddressMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);