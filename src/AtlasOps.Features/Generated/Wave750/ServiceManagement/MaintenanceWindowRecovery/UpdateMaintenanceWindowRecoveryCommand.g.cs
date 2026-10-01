namespace AtlasOps.Features.ServiceManagement.MaintenanceWindowRecovery;

public sealed record UpdateMaintenanceWindowRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);