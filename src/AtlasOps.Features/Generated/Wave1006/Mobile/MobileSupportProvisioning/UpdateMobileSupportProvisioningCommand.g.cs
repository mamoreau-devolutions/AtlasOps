namespace AtlasOps.Features.Mobile.MobileSupportProvisioning;

public sealed record UpdateMobileSupportProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);