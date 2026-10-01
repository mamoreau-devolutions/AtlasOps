namespace AtlasOps.Features.Storage.StorageArchiveGovernance;

public sealed record UpdateStorageArchiveGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);