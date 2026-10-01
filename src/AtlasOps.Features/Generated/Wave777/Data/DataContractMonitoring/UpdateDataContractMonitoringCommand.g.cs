namespace AtlasOps.Features.Data.DataContractMonitoring;

public sealed record UpdateDataContractMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);