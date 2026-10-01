namespace AtlasOps.Features.Database.DatabaseBackupRecovery;

public sealed record UpdateDatabaseBackupRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);