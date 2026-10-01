namespace AtlasOps.Features.Database.DatabaseSchemaProvisioning;

public sealed record UpdateDatabaseSchemaProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);