namespace AtlasOps.Features.Desktop.DesktopHealthOptimization;

public sealed record UpdateDesktopHealthOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);