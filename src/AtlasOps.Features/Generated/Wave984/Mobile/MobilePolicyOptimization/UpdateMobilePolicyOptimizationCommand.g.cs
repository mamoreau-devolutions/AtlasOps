namespace AtlasOps.Features.Mobile.MobilePolicyOptimization;

public sealed record UpdateMobilePolicyOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);