namespace AtlasOps.Features.Data.DataLineageMonitoring;

public sealed record UpdateDataLineageMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);