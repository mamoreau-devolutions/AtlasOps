namespace AtlasOps.Features.Compute.ComputeConsoleMonitoring;

public sealed record ComputeConsoleMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);