namespace AtlasOps.Features.Security.SecurityCertificateProvisioning;

public sealed record UpdateSecurityCertificateProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);