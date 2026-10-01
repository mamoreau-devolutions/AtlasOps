namespace AtlasOps.Features.Compute.ComputeImageMonitoring;

public sealed record ComputeImageMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);