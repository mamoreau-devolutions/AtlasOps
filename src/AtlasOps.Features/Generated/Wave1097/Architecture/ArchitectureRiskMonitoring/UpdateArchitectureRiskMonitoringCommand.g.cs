namespace AtlasOps.Features.Architecture.ArchitectureRiskMonitoring;

public sealed record UpdateArchitectureRiskMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);