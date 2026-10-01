namespace AtlasOps.Features.Database.NoSqlDatabaseProvisioning;

public sealed record UpdateNoSqlDatabaseProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);