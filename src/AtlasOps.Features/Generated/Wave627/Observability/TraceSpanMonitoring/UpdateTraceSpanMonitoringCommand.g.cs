namespace AtlasOps.Features.Observability.TraceSpanMonitoring;

public sealed record UpdateTraceSpanMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);