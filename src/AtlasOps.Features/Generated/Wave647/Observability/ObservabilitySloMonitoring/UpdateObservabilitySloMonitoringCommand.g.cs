namespace AtlasOps.Features.Observability.ObservabilitySloMonitoring;

public sealed record UpdateObservabilitySloMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);