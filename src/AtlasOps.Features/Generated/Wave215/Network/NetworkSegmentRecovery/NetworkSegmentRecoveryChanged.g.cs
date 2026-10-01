namespace AtlasOps.Features.Network.NetworkSegmentRecovery;

public sealed record NetworkSegmentRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);