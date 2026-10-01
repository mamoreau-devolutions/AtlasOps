namespace AtlasOps.Features.Messaging.MessageSchemaRecovery;

public sealed record MessageSchemaRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);