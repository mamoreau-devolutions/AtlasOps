namespace AtlasOps.Features.Compute.ComputeImageMonitoring;

public sealed record UpdateComputeImageMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);