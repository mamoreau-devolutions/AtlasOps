namespace AtlasOps.Features.Cloud.AwsAccountGovernance;

public sealed record UpdateAwsAccountGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);