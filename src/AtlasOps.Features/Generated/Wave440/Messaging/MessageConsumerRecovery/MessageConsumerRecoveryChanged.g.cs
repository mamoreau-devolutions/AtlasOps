namespace AtlasOps.Features.Messaging.MessageConsumerRecovery;

public sealed record MessageConsumerRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);