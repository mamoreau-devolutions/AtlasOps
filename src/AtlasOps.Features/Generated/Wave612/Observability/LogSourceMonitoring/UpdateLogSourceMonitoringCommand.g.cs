namespace AtlasOps.Features.Observability.LogSourceMonitoring;

public sealed record UpdateLogSourceMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);