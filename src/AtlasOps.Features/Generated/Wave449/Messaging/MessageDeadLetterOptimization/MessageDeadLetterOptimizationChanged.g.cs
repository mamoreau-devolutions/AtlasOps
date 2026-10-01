namespace AtlasOps.Features.Messaging.MessageDeadLetterOptimization;

public sealed record MessageDeadLetterOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);