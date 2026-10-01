namespace AtlasOps.Features.Desktop.DesktopUpdateOptimization;

public sealed record UpdateDesktopUpdateOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);