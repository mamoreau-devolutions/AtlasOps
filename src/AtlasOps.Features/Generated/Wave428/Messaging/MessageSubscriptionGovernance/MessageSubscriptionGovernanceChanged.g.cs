namespace AtlasOps.Features.Messaging.MessageSubscriptionGovernance;

public sealed record MessageSubscriptionGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);