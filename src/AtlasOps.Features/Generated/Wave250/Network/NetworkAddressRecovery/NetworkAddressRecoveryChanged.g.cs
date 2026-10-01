namespace AtlasOps.Features.Network.NetworkAddressRecovery;

public sealed record NetworkAddressRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);