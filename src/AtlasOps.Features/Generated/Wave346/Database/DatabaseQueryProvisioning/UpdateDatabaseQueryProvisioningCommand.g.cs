namespace AtlasOps.Features.Database.DatabaseQueryProvisioning;

public sealed record UpdateDatabaseQueryProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);