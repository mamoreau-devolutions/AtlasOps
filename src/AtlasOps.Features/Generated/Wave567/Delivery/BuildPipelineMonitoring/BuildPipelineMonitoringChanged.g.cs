namespace AtlasOps.Features.Delivery.BuildPipelineMonitoring;

public sealed record BuildPipelineMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);