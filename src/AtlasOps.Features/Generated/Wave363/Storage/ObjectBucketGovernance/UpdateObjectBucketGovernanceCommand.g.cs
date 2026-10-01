namespace AtlasOps.Features.Storage.ObjectBucketGovernance;

public sealed record UpdateObjectBucketGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);