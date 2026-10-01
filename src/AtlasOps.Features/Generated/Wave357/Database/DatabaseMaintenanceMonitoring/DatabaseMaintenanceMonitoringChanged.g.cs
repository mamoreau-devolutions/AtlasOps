namespace AtlasOps.Features.Database.DatabaseMaintenanceMonitoring;

public sealed record DatabaseMaintenanceMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);