namespace AtlasOps.Features.Network.NetworkPeerGovernance;

public sealed record NetworkPeerGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);