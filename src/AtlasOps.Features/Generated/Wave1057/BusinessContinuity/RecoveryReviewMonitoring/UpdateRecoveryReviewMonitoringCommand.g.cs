namespace AtlasOps.Features.BusinessContinuity.RecoveryReviewMonitoring;

public sealed record UpdateRecoveryReviewMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);