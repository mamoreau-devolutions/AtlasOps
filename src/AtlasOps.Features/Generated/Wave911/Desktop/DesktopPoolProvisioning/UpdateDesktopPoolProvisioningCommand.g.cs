namespace AtlasOps.Features.Desktop.DesktopPoolProvisioning;

public sealed record UpdateDesktopPoolProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);