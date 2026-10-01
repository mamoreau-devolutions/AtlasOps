namespace AtlasOps.Features.ServiceManagement.ServiceScorecardMonitoring;

public sealed record UpdateServiceScorecardMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);