namespace AtlasOps.Features.Desktop.DesktopPolicyOptimization;

public sealed record UpdateDesktopPolicyOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);