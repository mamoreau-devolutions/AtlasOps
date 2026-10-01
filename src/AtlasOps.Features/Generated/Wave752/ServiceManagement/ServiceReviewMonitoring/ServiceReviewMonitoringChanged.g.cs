namespace AtlasOps.Features.ServiceManagement.ServiceReviewMonitoring;

public sealed record ServiceReviewMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);