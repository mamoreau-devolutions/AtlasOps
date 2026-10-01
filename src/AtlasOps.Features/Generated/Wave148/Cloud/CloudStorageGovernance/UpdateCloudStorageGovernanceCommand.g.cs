namespace AtlasOps.Features.Cloud.CloudStorageGovernance;

public sealed record UpdateCloudStorageGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);