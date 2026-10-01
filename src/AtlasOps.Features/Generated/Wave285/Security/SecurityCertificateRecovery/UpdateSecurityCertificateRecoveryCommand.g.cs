namespace AtlasOps.Features.Security.SecurityCertificateRecovery;

public sealed record UpdateSecurityCertificateRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);