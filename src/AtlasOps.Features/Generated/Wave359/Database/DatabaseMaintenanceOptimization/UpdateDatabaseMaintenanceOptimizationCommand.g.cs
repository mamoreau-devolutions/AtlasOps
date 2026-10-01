namespace AtlasOps.Features.Database.DatabaseMaintenanceOptimization;

public sealed record UpdateDatabaseMaintenanceOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);