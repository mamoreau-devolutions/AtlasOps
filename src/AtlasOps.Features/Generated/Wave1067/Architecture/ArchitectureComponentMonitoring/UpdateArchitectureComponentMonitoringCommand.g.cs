namespace AtlasOps.Features.Architecture.ArchitectureComponentMonitoring;

public sealed record UpdateArchitectureComponentMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);