namespace AtlasOps.Features.Delivery.ReleaseApprovalOptimization;

public sealed record UpdateReleaseApprovalOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);