namespace AtlasOps.Features.ServiceManagement.ServiceCatalogMonitoring;

public sealed record UpdateServiceCatalogMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);