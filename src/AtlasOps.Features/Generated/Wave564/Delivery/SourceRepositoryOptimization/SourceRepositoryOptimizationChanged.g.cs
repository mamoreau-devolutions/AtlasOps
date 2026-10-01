namespace AtlasOps.Features.Delivery.SourceRepositoryOptimization;

public sealed record SourceRepositoryOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);