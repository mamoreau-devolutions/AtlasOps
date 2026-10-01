namespace AtlasOps.Features.Database.DatabaseReplicaOptimization;

public sealed record UpdateDatabaseReplicaOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);