namespace AtlasOps.Features.ServiceManagement.MaintenanceWindowMonitoring;

public sealed record MaintenanceWindowMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);