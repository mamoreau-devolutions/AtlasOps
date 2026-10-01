namespace AtlasOps.Features.Messaging.MessageQueueRecovery;

public sealed record MessageQueueRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);