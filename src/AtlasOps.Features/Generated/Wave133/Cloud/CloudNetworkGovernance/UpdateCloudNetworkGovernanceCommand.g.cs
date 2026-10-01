namespace AtlasOps.Features.Cloud.CloudNetworkGovernance;

public sealed record UpdateCloudNetworkGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);