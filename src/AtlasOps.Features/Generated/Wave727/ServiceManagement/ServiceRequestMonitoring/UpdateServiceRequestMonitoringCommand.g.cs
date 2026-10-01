namespace AtlasOps.Features.ServiceManagement.ServiceRequestMonitoring;

public sealed record UpdateServiceRequestMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);