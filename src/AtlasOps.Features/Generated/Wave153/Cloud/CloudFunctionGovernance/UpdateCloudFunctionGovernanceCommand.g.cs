namespace AtlasOps.Features.Cloud.CloudFunctionGovernance;

public sealed record UpdateCloudFunctionGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);