namespace AtlasOps.Features.Storage.StorageArchiveOptimization;

public sealed record UpdateStorageArchiveOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);