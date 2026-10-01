namespace AtlasOps.Features.Compute.ComputeLifecycleMonitoring;

public sealed record ComputeLifecycleMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);