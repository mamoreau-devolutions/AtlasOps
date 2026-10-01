namespace AtlasOps.Features.Messaging.MessageQueueOptimization;

public sealed record MessageQueueOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);