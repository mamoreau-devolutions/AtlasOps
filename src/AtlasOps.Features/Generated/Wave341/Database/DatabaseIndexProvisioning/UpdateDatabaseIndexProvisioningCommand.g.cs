namespace AtlasOps.Features.Database.DatabaseIndexProvisioning;

public sealed record UpdateDatabaseIndexProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);