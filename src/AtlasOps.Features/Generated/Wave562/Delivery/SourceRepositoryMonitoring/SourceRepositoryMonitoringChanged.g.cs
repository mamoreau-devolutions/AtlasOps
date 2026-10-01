namespace AtlasOps.Features.Delivery.SourceRepositoryMonitoring;

public sealed record SourceRepositoryMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);