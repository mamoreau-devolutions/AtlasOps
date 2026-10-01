namespace AtlasOps.Features.BusinessContinuity.RecoveryReviewMonitoring;

public sealed record RecoveryReviewMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);