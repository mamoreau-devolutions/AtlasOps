namespace AtlasOps.Features.Desktop.DesktopProfileOptimization;

public sealed record UpdateDesktopProfileOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);