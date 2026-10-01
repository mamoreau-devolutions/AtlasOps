namespace AtlasOps.Features.Storage.StorageLifecycleGovernance;

public sealed record UpdateStorageLifecycleGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);