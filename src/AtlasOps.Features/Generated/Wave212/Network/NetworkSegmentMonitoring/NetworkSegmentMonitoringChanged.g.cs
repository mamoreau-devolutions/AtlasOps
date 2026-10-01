namespace AtlasOps.Features.Network.NetworkSegmentMonitoring;

public sealed record NetworkSegmentMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);