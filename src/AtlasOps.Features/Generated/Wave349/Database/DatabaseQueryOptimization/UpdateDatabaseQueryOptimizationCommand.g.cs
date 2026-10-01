namespace AtlasOps.Features.Database.DatabaseQueryOptimization;

public sealed record UpdateDatabaseQueryOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);