namespace AtlasOps.Features.ServiceManagement.ServiceDependencyMonitoring;

public sealed record UpdateServiceDependencyMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);