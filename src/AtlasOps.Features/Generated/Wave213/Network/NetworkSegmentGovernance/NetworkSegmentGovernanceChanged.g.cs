namespace AtlasOps.Features.Network.NetworkSegmentGovernance;

public sealed record NetworkSegmentGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);