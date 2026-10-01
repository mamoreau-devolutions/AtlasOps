namespace AtlasOps.Features.Mobile.MobileComplianceProvisioning;

public sealed record UpdateMobileComplianceProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);