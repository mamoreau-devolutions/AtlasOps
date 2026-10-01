namespace AtlasOps.Features.BusinessContinuity.RecoverySiteOptimization;

public sealed record UpdateRecoverySiteOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);