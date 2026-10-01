namespace AtlasOps.Features.Cloud.CloudBillingGovernance;

public sealed record UpdateCloudBillingGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);