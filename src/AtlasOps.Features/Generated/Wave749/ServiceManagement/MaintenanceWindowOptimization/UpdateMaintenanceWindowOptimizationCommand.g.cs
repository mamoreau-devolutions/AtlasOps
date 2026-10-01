namespace AtlasOps.Features.ServiceManagement.MaintenanceWindowOptimization;

public sealed record UpdateMaintenanceWindowOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);