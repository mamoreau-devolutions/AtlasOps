namespace AtlasOps.Features.BusinessContinuity.RecoverySiteMonitoring;

public sealed record UpdateRecoverySiteMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);