namespace AtlasOps.Features.Messaging.MessageProducerOptimization;

public sealed record MessageProducerOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);