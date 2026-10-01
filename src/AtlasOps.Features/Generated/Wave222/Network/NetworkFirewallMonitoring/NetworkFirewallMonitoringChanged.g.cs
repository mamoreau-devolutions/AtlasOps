namespace AtlasOps.Features.Network.NetworkFirewallMonitoring;

public sealed record NetworkFirewallMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);