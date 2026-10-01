namespace AtlasOps.Features.Network.NetworkFirewallGovernance;

public sealed record NetworkFirewallGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);