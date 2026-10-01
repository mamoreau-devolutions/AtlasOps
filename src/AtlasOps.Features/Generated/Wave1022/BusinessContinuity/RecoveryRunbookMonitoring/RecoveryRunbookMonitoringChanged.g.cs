namespace AtlasOps.Features.BusinessContinuity.RecoveryRunbookMonitoring;

public sealed record RecoveryRunbookMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);