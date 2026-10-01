namespace AtlasOps.Features.Messaging.MessageTopicRecovery;

public sealed record MessageTopicRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);