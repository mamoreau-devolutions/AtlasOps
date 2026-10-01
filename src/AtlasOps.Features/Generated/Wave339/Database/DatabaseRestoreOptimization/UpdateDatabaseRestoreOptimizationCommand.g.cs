namespace AtlasOps.Features.Database.DatabaseRestoreOptimization;

public sealed record UpdateDatabaseRestoreOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);