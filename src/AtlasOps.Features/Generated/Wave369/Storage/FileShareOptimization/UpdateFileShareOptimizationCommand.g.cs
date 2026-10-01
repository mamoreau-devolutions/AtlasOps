namespace AtlasOps.Features.Storage.FileShareOptimization;

public sealed record UpdateFileShareOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);