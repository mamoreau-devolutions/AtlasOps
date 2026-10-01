namespace AtlasOps.Features.ServiceManagement.ServiceReviewOptimization;

public sealed record ServiceReviewOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);