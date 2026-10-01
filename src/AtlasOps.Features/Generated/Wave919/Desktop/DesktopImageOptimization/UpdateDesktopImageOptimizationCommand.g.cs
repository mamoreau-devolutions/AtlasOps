namespace AtlasOps.Features.Desktop.DesktopImageOptimization;

public sealed record UpdateDesktopImageOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);