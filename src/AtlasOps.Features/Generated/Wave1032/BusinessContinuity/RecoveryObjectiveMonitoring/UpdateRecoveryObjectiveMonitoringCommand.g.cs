namespace AtlasOps.Features.BusinessContinuity.RecoveryObjectiveMonitoring;

public sealed record UpdateRecoveryObjectiveMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);