namespace AtlasOps.Features.Messaging.MessageReplayRecovery;

public sealed record MessageReplayRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);