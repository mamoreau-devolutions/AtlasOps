namespace AtlasOps.Features.Data.DataProductMonitoring;

public sealed record UpdateDataProductMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);