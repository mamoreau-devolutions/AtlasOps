namespace AtlasOps.Features.Mobile.MobileSupportOptimization;

public sealed record UpdateMobileSupportOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);