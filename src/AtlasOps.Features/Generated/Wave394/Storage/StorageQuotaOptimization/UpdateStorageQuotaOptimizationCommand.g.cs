namespace AtlasOps.Features.Storage.StorageQuotaOptimization;

public sealed record UpdateStorageQuotaOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);