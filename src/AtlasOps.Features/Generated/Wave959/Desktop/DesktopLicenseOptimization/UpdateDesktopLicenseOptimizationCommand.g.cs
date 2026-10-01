namespace AtlasOps.Features.Desktop.DesktopLicenseOptimization;

public sealed record UpdateDesktopLicenseOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);