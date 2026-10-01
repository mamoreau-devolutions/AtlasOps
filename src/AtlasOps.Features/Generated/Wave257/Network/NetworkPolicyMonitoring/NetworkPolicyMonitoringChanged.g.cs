namespace AtlasOps.Features.Network.NetworkPolicyMonitoring;

public sealed record NetworkPolicyMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);