namespace AtlasOps.Features.Messaging.MessageSubscriptionRecovery;

public sealed record MessageSubscriptionRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);