namespace AtlasOps.Features.Mobile.MobileApplicationProvisioning;

public sealed record UpdateMobileApplicationProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);