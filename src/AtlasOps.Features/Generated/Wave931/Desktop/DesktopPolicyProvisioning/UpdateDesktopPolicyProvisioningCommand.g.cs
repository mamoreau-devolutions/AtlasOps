namespace AtlasOps.Features.Desktop.DesktopPolicyProvisioning;

public sealed record UpdateDesktopPolicyProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);