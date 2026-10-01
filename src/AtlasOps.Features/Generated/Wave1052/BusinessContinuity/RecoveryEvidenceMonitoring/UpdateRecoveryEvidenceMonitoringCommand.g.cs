namespace AtlasOps.Features.BusinessContinuity.RecoveryEvidenceMonitoring;

public sealed record UpdateRecoveryEvidenceMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);