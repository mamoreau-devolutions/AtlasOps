namespace AtlasOps.Features.Architecture.ArchitectureExceptionMonitoring;

public sealed record UpdateArchitectureExceptionMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);