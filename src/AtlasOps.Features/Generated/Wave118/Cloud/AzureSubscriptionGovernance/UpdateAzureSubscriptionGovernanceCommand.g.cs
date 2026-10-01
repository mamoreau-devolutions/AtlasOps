namespace AtlasOps.Features.Cloud.AzureSubscriptionGovernance;

public sealed record UpdateAzureSubscriptionGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);