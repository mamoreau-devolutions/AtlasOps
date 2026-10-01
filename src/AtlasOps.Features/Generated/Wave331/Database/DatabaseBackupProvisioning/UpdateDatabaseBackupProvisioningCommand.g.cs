namespace AtlasOps.Features.Database.DatabaseBackupProvisioning;

public sealed record UpdateDatabaseBackupProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);