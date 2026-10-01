namespace AtlasOps.Features.Database.DatabaseSchemaRecovery;

public sealed record DatabaseSchemaRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);