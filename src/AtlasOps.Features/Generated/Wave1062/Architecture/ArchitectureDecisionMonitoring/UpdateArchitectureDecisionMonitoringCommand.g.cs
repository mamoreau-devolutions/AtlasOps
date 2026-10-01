namespace AtlasOps.Features.Architecture.ArchitectureDecisionMonitoring;

public sealed record UpdateArchitectureDecisionMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);