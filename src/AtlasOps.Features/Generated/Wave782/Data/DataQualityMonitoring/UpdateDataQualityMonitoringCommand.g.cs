namespace AtlasOps.Features.Data.DataQualityMonitoring;

public sealed record UpdateDataQualityMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);