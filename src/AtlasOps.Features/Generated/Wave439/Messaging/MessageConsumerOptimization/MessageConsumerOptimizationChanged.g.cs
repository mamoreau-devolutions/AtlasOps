namespace AtlasOps.Features.Messaging.MessageConsumerOptimization;

public sealed record MessageConsumerOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);