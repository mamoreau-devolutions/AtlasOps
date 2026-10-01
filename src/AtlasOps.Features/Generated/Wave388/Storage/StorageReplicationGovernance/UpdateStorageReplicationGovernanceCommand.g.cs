namespace AtlasOps.Features.Storage.StorageReplicationGovernance;

public sealed record UpdateStorageReplicationGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);