namespace AtlasOps.Features.Desktop.DesktopApplicationOptimization;

public sealed record UpdateDesktopApplicationOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);