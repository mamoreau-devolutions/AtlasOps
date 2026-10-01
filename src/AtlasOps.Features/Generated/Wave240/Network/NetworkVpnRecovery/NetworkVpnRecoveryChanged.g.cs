namespace AtlasOps.Features.Network.NetworkVpnRecovery;

public sealed record NetworkVpnRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);