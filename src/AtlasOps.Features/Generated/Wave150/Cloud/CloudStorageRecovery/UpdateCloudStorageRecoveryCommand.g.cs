namespace AtlasOps.Features.Cloud.CloudStorageRecovery;

public sealed record UpdateCloudStorageRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);