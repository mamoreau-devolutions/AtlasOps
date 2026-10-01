namespace AtlasOps.Features.Mobile.MobileFleetOptimization;

public sealed record UpdateMobileFleetOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);