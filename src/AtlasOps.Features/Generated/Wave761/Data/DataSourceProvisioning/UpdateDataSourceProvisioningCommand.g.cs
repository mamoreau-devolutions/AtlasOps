namespace AtlasOps.Features.Data.DataSourceProvisioning;

public sealed record UpdateDataSourceProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);