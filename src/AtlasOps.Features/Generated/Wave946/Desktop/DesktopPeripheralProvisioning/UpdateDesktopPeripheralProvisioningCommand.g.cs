namespace AtlasOps.Features.Desktop.DesktopPeripheralProvisioning;

public sealed record UpdateDesktopPeripheralProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);