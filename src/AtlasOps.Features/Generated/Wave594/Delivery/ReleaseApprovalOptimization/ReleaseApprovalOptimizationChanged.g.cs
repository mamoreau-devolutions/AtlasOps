namespace AtlasOps.Features.Delivery.ReleaseApprovalOptimization;

public sealed record ReleaseApprovalOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);