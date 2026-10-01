namespace AtlasOps.Features.Mobile.MobileFleetProvisioning;

public sealed record UpdateMobileFleetProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);