namespace AtlasOps.Features.Mobile.MobileProfileProvisioning;

public sealed record UpdateMobileProfileProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);