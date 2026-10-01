namespace AtlasOps.Features.Database.DatabaseSchemaMonitoring;

public sealed record UpdateDatabaseSchemaMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);