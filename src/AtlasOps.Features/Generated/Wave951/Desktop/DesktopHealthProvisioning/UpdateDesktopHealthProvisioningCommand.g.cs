namespace AtlasOps.Features.Desktop.DesktopHealthProvisioning;

public sealed record UpdateDesktopHealthProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);