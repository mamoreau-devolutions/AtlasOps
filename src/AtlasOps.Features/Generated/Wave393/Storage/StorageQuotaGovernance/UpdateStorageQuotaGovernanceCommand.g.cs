namespace AtlasOps.Features.Storage.StorageQuotaGovernance;

public sealed record UpdateStorageQuotaGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);