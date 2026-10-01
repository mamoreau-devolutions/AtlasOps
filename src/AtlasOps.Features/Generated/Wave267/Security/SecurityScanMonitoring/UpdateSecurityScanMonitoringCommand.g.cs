namespace AtlasOps.Features.Security.SecurityScanMonitoring;

public sealed record UpdateSecurityScanMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);