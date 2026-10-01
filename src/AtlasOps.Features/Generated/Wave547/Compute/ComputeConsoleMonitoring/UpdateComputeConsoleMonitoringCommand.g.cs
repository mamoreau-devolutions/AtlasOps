namespace AtlasOps.Features.Compute.ComputeConsoleMonitoring;

public sealed record UpdateComputeConsoleMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);