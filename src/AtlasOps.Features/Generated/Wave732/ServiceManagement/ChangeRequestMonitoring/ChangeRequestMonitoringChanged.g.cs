namespace AtlasOps.Features.ServiceManagement.ChangeRequestMonitoring;

public sealed record ChangeRequestMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);