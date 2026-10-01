namespace AtlasOps.Features.Network.NetworkAddressGovernance;

public sealed record NetworkAddressGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);