namespace AtlasOps.Features.Messaging.MessageRetentionRecovery;

public sealed record MessageRetentionRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);