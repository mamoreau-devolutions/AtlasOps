namespace AtlasOps.Features.Network.NetworkFirewallOptimization;

public sealed record NetworkFirewallOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);