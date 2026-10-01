namespace AtlasOps.Features.Network.NetworkVpnGovernance;

public sealed record NetworkVpnGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);