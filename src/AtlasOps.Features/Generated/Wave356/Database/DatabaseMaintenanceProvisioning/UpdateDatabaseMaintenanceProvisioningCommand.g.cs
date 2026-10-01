namespace AtlasOps.Features.Database.DatabaseMaintenanceProvisioning;

public sealed record UpdateDatabaseMaintenanceProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);