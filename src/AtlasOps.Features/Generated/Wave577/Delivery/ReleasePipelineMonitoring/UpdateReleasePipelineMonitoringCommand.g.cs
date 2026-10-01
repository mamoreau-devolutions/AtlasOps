namespace AtlasOps.Features.Delivery.ReleasePipelineMonitoring;

public sealed record UpdateReleasePipelineMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);