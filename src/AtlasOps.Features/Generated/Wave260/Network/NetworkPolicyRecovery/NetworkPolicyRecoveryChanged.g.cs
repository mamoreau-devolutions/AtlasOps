namespace AtlasOps.Features.Network.NetworkPolicyRecovery;

public sealed record NetworkPolicyRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);