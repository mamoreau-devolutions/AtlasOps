namespace AtlasOps.Features.ServiceManagement.MaintenanceWindowProvisioning;

public sealed record UpdateMaintenanceWindowProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);