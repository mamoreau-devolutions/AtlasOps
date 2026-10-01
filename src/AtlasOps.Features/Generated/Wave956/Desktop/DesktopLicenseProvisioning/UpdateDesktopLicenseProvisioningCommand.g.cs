namespace AtlasOps.Features.Desktop.DesktopLicenseProvisioning;

public sealed record UpdateDesktopLicenseProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);