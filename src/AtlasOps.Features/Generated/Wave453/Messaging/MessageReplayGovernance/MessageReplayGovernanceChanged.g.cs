namespace AtlasOps.Features.Messaging.MessageReplayGovernance;

public sealed record MessageReplayGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);