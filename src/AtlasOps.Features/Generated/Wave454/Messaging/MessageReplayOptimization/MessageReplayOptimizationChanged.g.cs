namespace AtlasOps.Features.Messaging.MessageReplayOptimization;

public sealed record MessageReplayOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);