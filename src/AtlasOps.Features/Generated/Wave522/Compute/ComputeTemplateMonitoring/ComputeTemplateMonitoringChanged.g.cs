namespace AtlasOps.Features.Compute.ComputeTemplateMonitoring;

public sealed record ComputeTemplateMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);