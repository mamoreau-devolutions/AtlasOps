namespace AtlasOps.Features.Database.DatabaseRestoreMonitoring;

public sealed record UpdateDatabaseRestoreMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);