namespace AtlasOps.Features.Network.NetworkPeerRecovery;

public sealed record NetworkPeerRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);