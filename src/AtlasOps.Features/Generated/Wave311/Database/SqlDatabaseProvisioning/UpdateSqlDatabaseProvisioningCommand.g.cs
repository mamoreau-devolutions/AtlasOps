namespace AtlasOps.Features.Database.SqlDatabaseProvisioning;

public sealed record UpdateSqlDatabaseProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);