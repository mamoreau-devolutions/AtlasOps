namespace AtlasOps.Features.Database.DatabaseQueryMonitoring;

public sealed record UpdateDatabaseQueryMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);