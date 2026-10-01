namespace AtlasOps.Features.Delivery.ReleaseRollbackOptimization;

public sealed record UpdateReleaseRollbackOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);