namespace AtlasOps.Features.Mobile.MobileCertificateProvisioning;

public sealed record UpdateMobileCertificateProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);