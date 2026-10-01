namespace AtlasOps.Features.Security.SecurityPatchMonitoring;

public sealed record UpdateSecurityPatchMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);