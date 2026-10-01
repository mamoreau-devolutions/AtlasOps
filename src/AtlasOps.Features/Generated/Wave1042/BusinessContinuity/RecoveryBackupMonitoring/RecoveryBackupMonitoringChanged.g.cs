namespace AtlasOps.Features.BusinessContinuity.RecoveryBackupMonitoring;

public sealed record RecoveryBackupMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);