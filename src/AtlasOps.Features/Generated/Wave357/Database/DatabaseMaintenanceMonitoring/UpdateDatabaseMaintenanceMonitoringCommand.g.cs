namespace AtlasOps.Features.Database.DatabaseMaintenanceMonitoring;

public sealed record UpdateDatabaseMaintenanceMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);