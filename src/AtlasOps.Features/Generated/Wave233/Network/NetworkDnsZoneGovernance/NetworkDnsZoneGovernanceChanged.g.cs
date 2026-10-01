namespace AtlasOps.Features.Network.NetworkDnsZoneGovernance;

public sealed record NetworkDnsZoneGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);