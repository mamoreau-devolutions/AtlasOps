namespace AtlasOps.Features.Messaging.MessageSubscriptionOptimization;

public sealed record MessageSubscriptionOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);