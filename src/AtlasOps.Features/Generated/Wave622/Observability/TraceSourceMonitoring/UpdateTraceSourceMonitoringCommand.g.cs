namespace AtlasOps.Features.Observability.TraceSourceMonitoring;

public sealed record UpdateTraceSourceMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);