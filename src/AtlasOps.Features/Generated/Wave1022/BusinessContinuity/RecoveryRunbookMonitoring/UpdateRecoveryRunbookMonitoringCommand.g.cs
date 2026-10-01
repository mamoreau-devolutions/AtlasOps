namespace AtlasOps.Features.BusinessContinuity.RecoveryRunbookMonitoring;

public sealed record UpdateRecoveryRunbookMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);