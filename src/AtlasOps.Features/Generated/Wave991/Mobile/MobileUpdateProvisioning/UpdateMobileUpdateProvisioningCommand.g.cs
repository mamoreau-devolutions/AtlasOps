namespace AtlasOps.Features.Mobile.MobileUpdateProvisioning;

public sealed record UpdateMobileUpdateProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);