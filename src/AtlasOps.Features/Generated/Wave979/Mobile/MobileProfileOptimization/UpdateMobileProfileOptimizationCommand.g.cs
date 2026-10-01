namespace AtlasOps.Features.Mobile.MobileProfileOptimization;

public sealed record UpdateMobileProfileOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);