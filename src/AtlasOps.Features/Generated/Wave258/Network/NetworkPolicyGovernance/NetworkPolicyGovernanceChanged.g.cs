namespace AtlasOps.Features.Network.NetworkPolicyGovernance;

public sealed record NetworkPolicyGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);