namespace AtlasOps.Features.Delivery.BuildPipelineMonitoring;

public sealed record UpdateBuildPipelineMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);