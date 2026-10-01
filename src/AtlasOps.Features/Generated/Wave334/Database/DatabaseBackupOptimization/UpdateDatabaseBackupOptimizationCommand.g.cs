namespace AtlasOps.Features.Database.DatabaseBackupOptimization;

public sealed record UpdateDatabaseBackupOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);