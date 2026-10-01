namespace AtlasOps.Features.Delivery.SourceRepositoryOptimization;

public sealed record UpdateSourceRepositoryOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);