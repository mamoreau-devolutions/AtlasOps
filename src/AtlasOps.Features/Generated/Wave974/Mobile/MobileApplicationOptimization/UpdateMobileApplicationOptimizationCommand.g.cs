namespace AtlasOps.Features.Mobile.MobileApplicationOptimization;

public sealed record UpdateMobileApplicationOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);