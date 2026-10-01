namespace AtlasOps.Features.Cloud.CloudDatabaseGovernance;

public sealed record UpdateCloudDatabaseGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);