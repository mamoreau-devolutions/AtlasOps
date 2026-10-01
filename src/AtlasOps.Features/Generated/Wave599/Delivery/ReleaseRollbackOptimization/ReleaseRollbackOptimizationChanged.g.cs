namespace AtlasOps.Features.Delivery.ReleaseRollbackOptimization;

public sealed record ReleaseRollbackOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);