namespace AtlasOps.Features.Architecture.ArchitectureReviewMonitoring;

public sealed record UpdateArchitectureReviewMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);