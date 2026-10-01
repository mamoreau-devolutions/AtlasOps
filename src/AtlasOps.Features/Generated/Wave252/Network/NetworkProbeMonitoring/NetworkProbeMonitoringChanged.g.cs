namespace AtlasOps.Features.Network.NetworkProbeMonitoring;

public sealed record NetworkProbeMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);