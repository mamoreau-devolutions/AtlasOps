namespace AtlasOps.Features.Desktop.DesktopSessionProvisioning;

public sealed record UpdateDesktopSessionProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);