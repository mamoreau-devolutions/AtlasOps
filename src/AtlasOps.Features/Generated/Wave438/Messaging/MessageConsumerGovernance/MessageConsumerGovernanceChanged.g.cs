namespace AtlasOps.Features.Messaging.MessageConsumerGovernance;

public sealed record MessageConsumerGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);