namespace AtlasOps.Features.BusinessContinuity.RecoveryEvidenceOptimization;

public sealed record RecoveryEvidenceOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);