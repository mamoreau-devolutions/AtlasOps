namespace AtlasOps.Features.Data.DataTransformMonitoring;

public sealed record UpdateDataTransformMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);