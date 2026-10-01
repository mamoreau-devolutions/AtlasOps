namespace AtlasOps.Features.ServiceManagement.ServiceCatalogMonitoring;

public sealed record ServiceCatalogMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);