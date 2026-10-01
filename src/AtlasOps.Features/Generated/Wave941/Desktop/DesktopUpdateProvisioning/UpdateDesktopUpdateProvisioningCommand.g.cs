namespace AtlasOps.Features.Desktop.DesktopUpdateProvisioning;

public sealed record UpdateDesktopUpdateProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);