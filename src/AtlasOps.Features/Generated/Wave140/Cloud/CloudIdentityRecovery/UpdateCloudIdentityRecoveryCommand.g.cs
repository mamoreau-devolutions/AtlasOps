namespace AtlasOps.Features.Cloud.CloudIdentityRecovery;

public sealed record UpdateCloudIdentityRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);