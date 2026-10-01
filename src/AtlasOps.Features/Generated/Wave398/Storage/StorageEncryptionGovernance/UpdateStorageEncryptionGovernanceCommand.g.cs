namespace AtlasOps.Features.Storage.StorageEncryptionGovernance;

public sealed record UpdateStorageEncryptionGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);