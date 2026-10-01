namespace AtlasOps.Features.Desktop.DesktopImageProvisioning;

public sealed record UpdateDesktopImageProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);