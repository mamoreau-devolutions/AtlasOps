namespace AtlasOps.Features.Cloud.CloudIdentityGovernance;

public sealed record UpdateCloudIdentityGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);