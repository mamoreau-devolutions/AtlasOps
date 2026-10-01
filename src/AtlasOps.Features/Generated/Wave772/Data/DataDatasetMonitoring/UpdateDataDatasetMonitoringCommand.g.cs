namespace AtlasOps.Features.Data.DataDatasetMonitoring;

public sealed record UpdateDataDatasetMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);