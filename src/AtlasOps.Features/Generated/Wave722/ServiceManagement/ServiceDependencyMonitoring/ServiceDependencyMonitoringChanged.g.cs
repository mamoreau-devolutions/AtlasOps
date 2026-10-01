namespace AtlasOps.Features.ServiceManagement.ServiceDependencyMonitoring;

public sealed record ServiceDependencyMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);