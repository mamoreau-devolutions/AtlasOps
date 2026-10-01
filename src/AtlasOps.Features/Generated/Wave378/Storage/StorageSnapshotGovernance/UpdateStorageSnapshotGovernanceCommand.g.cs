namespace AtlasOps.Features.Storage.StorageSnapshotGovernance;

public sealed record UpdateStorageSnapshotGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);