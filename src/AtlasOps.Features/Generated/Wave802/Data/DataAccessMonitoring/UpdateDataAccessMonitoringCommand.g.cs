namespace AtlasOps.Features.Data.DataAccessMonitoring;

public sealed record UpdateDataAccessMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);