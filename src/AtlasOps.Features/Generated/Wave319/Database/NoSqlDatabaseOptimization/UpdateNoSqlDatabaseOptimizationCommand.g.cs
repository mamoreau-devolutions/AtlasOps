namespace AtlasOps.Features.Database.NoSqlDatabaseOptimization;

public sealed record UpdateNoSqlDatabaseOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);