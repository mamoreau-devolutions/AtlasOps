namespace AtlasOps.Features.Security.SecurityCertificateGovernance;

public sealed record UpdateSecurityCertificateGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);