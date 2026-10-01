namespace AtlasOps.Features.BusinessContinuity.RecoveryFailoverMonitoring;

public sealed record UpdateRecoveryFailoverMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);