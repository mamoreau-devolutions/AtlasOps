namespace AtlasOps.Features.Database.DatabaseReplicaMonitoring;

public sealed record UpdateDatabaseReplicaMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);