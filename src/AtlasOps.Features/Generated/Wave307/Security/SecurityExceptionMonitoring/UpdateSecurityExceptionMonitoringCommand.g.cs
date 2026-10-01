namespace AtlasOps.Features.Security.SecurityExceptionMonitoring;

public sealed record UpdateSecurityExceptionMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);