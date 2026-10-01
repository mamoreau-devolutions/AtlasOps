namespace AtlasOps.Features.Desktop.DesktopPeripheralOptimization;

public sealed record UpdateDesktopPeripheralOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);