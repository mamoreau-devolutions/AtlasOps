namespace AtlasOps.Features.Messaging.MessageQueueGovernance;

public sealed record MessageQueueGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);