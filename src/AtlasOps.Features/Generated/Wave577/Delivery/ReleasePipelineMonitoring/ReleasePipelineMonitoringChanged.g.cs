namespace AtlasOps.Features.Delivery.ReleasePipelineMonitoring;

public sealed record ReleasePipelineMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);