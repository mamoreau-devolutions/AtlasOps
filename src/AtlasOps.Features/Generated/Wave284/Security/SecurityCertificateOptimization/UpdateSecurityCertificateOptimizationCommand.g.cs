namespace AtlasOps.Features.Security.SecurityCertificateOptimization;

public sealed record UpdateSecurityCertificateOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);