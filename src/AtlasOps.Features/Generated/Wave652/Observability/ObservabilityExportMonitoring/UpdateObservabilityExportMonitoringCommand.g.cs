namespace AtlasOps.Features.Observability.ObservabilityExportMonitoring;

public sealed record UpdateObservabilityExportMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);