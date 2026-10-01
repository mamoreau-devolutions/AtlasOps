namespace AtlasOps.Features.Database.SqlDatabaseRecovery;

public sealed record UpdateSqlDatabaseRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);