namespace AtlasOps.Features.Network.NetworkFirewallRecovery;

public sealed record NetworkFirewallRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);