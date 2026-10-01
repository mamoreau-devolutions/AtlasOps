namespace AtlasOps.Features.Data.DataPipelineMonitoring;

public sealed record UpdateDataPipelineMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);