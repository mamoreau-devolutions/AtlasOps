namespace AtlasOps.Features.Data.DataAccessProvisioning;

public sealed record UpdateDataAccessProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);