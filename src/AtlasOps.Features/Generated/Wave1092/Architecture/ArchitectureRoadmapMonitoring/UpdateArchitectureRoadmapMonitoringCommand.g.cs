namespace AtlasOps.Features.Architecture.ArchitectureRoadmapMonitoring;

public sealed record UpdateArchitectureRoadmapMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);