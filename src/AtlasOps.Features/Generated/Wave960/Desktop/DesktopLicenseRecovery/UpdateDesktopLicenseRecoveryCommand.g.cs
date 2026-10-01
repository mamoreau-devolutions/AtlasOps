namespace AtlasOps.Features.Desktop.DesktopLicenseRecovery;

public sealed record UpdateDesktopLicenseRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);