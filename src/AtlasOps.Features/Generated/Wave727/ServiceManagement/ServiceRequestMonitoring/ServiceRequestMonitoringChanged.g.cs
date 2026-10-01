namespace AtlasOps.Features.ServiceManagement.ServiceRequestMonitoring;

public sealed record ServiceRequestMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);