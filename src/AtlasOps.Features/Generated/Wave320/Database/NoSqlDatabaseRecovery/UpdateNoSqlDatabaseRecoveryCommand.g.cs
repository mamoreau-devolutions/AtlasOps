namespace AtlasOps.Features.Database.NoSqlDatabaseRecovery;

public sealed record UpdateNoSqlDatabaseRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);