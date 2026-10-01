namespace AtlasOps.Features.Cloud.CloudNetworkRecovery;

public sealed record UpdateCloudNetworkRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);