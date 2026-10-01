namespace AtlasOps.Features.Mobile.MobilePolicyProvisioning;

public sealed record UpdateMobilePolicyProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);