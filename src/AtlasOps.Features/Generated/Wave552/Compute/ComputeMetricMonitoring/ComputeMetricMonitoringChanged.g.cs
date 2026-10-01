namespace AtlasOps.Features.Compute.ComputeMetricMonitoring;

public sealed record ComputeMetricMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);