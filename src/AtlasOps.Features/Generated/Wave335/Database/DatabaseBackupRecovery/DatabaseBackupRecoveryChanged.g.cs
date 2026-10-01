namespace AtlasOps.Features.Database.DatabaseBackupRecovery;

public sealed record DatabaseBackupRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);