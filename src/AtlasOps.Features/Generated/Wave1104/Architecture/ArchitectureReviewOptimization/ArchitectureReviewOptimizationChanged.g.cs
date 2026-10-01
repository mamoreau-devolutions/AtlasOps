namespace AtlasOps.Features.Architecture.ArchitectureReviewOptimization;

public sealed record ArchitectureReviewOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);