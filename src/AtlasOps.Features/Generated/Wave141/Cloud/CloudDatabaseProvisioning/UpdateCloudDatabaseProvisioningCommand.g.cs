namespace AtlasOps.Features.Cloud.CloudDatabaseProvisioning;

public sealed record UpdateCloudDatabaseProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);