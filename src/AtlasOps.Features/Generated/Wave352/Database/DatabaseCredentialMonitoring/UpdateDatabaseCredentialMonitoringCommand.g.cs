namespace AtlasOps.Features.Database.DatabaseCredentialMonitoring;

public sealed record UpdateDatabaseCredentialMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);