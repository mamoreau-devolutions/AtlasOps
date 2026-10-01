namespace AtlasOps.Features.ServiceManagement.ServiceOwnerMonitoring;

public sealed record ServiceOwnerMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);