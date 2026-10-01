namespace AtlasOps.Features.Cloud.CloudFunctionRecovery;

public sealed record UpdateCloudFunctionRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);