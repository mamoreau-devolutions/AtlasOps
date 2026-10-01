namespace AtlasOps.Features.Database.DatabaseIndexMonitoring;

public sealed record UpdateDatabaseIndexMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);