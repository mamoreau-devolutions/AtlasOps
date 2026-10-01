namespace AtlasOps.Features.Data.DataSourceMonitoring;

public sealed record UpdateDataSourceMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);