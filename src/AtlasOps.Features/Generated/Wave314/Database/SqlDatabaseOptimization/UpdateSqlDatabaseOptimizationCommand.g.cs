namespace AtlasOps.Features.Database.SqlDatabaseOptimization;

public sealed record UpdateSqlDatabaseOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);