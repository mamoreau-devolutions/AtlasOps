namespace AtlasOps.Features.Desktop.DesktopApplicationProvisioning;

public sealed record UpdateDesktopApplicationProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);