namespace AtlasOps.Features.BusinessContinuity.RecoveryEvidenceMonitoring;

public sealed record RecoveryEvidenceMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);