namespace AtlasOps.Features.Database.NoSqlDatabaseMonitoring;

public sealed record UpdateNoSqlDatabaseMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);