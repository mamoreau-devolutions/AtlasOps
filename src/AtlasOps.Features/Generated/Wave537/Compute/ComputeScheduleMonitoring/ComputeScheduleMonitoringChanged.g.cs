namespace AtlasOps.Features.Compute.ComputeScheduleMonitoring;

public sealed record ComputeScheduleMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);