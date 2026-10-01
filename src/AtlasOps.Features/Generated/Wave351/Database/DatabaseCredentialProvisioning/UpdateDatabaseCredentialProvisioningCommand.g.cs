namespace AtlasOps.Features.Database.DatabaseCredentialProvisioning;

public sealed record UpdateDatabaseCredentialProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);