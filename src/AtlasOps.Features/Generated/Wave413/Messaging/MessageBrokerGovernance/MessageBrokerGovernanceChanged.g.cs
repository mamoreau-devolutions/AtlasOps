namespace AtlasOps.Features.Messaging.MessageBrokerGovernance;

public sealed record MessageBrokerGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);