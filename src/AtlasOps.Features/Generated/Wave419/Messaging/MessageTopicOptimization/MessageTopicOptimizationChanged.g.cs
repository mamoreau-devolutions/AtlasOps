namespace AtlasOps.Features.Messaging.MessageTopicOptimization;

public sealed record MessageTopicOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);