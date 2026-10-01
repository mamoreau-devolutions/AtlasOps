namespace AtlasOps.Features.Mobile.MobileDeviceProvisioning;

public sealed record UpdateMobileDeviceProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);