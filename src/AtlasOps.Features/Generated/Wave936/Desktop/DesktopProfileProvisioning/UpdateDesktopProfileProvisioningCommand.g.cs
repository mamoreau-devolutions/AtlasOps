namespace AtlasOps.Features.Desktop.DesktopProfileProvisioning;

public sealed record UpdateDesktopProfileProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);