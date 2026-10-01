namespace AtlasOps.Features.FinOps.ResourceCommitmentOptimization;

public sealed record ResourceCommitmentOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);