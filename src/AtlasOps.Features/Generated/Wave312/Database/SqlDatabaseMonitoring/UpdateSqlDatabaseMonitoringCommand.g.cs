namespace AtlasOps.Features.Database.SqlDatabaseMonitoring;

public sealed record UpdateSqlDatabaseMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);