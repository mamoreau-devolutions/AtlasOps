namespace AtlasOps.Features.Messaging.MessageProducerRecovery;

public sealed record MessageProducerRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);