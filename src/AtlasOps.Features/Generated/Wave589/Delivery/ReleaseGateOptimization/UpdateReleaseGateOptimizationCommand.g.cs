namespace AtlasOps.Features.Delivery.ReleaseGateOptimization;

public sealed record UpdateReleaseGateOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);