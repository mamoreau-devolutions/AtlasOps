namespace AtlasOps.Features.Mobile.MobileUpdateOptimization;

public sealed record UpdateMobileUpdateOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);