namespace AtlasOps.Features.Database.DatabaseRestoreProvisioning;

public sealed record UpdateDatabaseRestoreProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);