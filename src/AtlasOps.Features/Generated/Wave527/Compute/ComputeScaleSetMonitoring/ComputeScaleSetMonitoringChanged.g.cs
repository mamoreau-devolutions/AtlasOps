namespace AtlasOps.Features.Compute.ComputeScaleSetMonitoring;

public sealed record ComputeScaleSetMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);