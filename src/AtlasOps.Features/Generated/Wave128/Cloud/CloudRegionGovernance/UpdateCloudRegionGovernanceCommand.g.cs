namespace AtlasOps.Features.Cloud.CloudRegionGovernance;

public sealed record UpdateCloudRegionGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);