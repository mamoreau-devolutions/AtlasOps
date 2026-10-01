namespace AtlasOps.Features.Messaging.MessageRetentionOptimization;

public sealed record MessageRetentionOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);