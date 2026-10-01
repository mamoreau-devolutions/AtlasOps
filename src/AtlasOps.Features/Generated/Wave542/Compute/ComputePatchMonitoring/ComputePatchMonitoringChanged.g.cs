namespace AtlasOps.Features.Compute.ComputePatchMonitoring;

public sealed record ComputePatchMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);