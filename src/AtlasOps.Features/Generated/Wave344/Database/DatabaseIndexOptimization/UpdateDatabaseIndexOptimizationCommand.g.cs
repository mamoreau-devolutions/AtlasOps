namespace AtlasOps.Features.Database.DatabaseIndexOptimization;

public sealed record UpdateDatabaseIndexOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);