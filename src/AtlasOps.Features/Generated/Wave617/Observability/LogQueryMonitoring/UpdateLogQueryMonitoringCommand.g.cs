namespace AtlasOps.Features.Observability.LogQueryMonitoring;

public sealed record UpdateLogQueryMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);