namespace AtlasOps.Features.Desktop.DesktopPoolOptimization;

public sealed record UpdateDesktopPoolOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);