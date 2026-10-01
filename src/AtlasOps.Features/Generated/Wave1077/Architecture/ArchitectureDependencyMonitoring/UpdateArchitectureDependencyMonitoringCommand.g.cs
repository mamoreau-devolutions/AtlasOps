namespace AtlasOps.Features.Architecture.ArchitectureDependencyMonitoring;

public sealed record UpdateArchitectureDependencyMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);