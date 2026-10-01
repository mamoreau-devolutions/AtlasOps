namespace AtlasOps.Features.Desktop.DesktopSessionOptimization;

public sealed record UpdateDesktopSessionOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);