namespace AtlasOps.Features.BusinessContinuity.RecoveryObjectiveMonitoring;

public sealed record RecoveryObjectiveMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);