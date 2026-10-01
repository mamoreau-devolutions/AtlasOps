namespace AtlasOps.Features.BusinessContinuity.RecoveryFailoverMonitoring;

public sealed record RecoveryFailoverMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);