namespace AtlasOps.Features.BusinessContinuity.RecoveryDependencyMonitoring;

public sealed record RecoveryDependencyMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);