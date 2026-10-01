namespace AtlasOps.Features.Storage.StorageTransferGovernance;

public sealed record UpdateStorageTransferGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);