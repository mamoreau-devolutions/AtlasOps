namespace AtlasOps.Features.Messaging.MessageTopicGovernance;

public sealed record MessageTopicGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);