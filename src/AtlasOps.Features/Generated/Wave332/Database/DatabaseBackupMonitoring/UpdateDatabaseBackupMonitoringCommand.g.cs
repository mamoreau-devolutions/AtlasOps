namespace AtlasOps.Features.Database.DatabaseBackupMonitoring;

public sealed record UpdateDatabaseBackupMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);