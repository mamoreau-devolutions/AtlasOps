namespace AtlasOps.Features.BusinessContinuity.RecoveryExerciseMonitoring;

public sealed record RecoveryExerciseMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);