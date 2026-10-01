namespace AtlasOps.Features.Observability.ObservabilityRetentionMonitoring;

public sealed record UpdateObservabilityRetentionMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);