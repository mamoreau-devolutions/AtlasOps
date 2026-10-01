namespace AtlasOps.Features.Mobile.MobileDeviceOptimization;

public sealed record UpdateMobileDeviceOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);