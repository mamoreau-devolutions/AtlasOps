namespace AtlasOps.Features.BusinessContinuity.RecoveryDependencyMonitoring;

public sealed record UpdateRecoveryDependencyMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);