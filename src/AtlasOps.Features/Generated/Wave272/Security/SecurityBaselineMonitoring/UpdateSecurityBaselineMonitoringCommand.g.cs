namespace AtlasOps.Features.Security.SecurityBaselineMonitoring;

public sealed record UpdateSecurityBaselineMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);