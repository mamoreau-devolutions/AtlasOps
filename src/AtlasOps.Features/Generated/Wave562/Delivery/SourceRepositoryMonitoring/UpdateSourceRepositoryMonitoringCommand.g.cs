namespace AtlasOps.Features.Delivery.SourceRepositoryMonitoring;

public sealed record UpdateSourceRepositoryMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);