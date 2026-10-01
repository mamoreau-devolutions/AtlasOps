namespace AtlasOps.Features.Messaging.MessageProducerGovernance;

public sealed record MessageProducerGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);