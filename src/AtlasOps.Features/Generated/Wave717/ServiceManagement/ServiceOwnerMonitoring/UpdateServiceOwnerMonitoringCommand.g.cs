namespace AtlasOps.Features.ServiceManagement.ServiceOwnerMonitoring;

public sealed record UpdateServiceOwnerMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);