namespace AtlasOps.Features.Messaging.MessageBrokerRecovery;

public sealed record MessageBrokerRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);