namespace AtlasOps.Features.Messaging.MessageDeadLetterRecovery;

public sealed record MessageDeadLetterRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);