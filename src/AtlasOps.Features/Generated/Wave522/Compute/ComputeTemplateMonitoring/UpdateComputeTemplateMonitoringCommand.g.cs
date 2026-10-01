namespace AtlasOps.Features.Compute.ComputeTemplateMonitoring;

public sealed record UpdateComputeTemplateMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);