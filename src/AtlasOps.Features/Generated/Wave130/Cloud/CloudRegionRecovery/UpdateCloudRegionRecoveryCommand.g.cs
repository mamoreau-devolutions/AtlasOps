namespace AtlasOps.Features.Cloud.CloudRegionRecovery;

public sealed record UpdateCloudRegionRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);