namespace AtlasOps.Features.Network.NetworkDnsZoneRecovery;

public sealed record NetworkDnsZoneRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);