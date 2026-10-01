namespace AtlasOps.Features.Security.SecurityCertificateMonitoring;

public sealed record UpdateSecurityCertificateMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);