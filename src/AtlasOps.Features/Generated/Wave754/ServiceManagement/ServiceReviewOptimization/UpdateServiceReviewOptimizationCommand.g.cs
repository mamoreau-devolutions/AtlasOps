namespace AtlasOps.Features.ServiceManagement.ServiceReviewOptimization;

public sealed record UpdateServiceReviewOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);