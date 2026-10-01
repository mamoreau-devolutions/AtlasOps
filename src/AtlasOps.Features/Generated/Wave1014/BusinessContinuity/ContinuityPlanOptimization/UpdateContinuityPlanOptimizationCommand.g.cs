namespace AtlasOps.Features.BusinessContinuity.ContinuityPlanOptimization;

public sealed record UpdateContinuityPlanOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);