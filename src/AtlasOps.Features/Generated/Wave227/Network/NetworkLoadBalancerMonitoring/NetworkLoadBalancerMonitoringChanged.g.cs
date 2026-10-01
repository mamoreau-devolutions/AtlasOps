namespace AtlasOps.Features.Network.NetworkLoadBalancerMonitoring;

public sealed record NetworkLoadBalancerMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);