namespace AtlasOps.Features.Architecture.ArchitectureStandardMonitoring;

public sealed record UpdateArchitectureStandardMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);