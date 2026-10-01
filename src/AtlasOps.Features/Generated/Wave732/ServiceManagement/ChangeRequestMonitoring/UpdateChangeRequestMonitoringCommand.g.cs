namespace AtlasOps.Features.ServiceManagement.ChangeRequestMonitoring;

public sealed record UpdateChangeRequestMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);