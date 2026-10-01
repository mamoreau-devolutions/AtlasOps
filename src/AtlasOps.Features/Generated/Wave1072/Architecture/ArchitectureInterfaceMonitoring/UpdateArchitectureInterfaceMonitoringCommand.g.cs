namespace AtlasOps.Features.Architecture.ArchitectureInterfaceMonitoring;

public sealed record UpdateArchitectureInterfaceMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);