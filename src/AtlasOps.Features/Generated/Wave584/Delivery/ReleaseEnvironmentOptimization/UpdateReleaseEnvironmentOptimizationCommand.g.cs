namespace AtlasOps.Features.Delivery.ReleaseEnvironmentOptimization;

public sealed record UpdateReleaseEnvironmentOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);