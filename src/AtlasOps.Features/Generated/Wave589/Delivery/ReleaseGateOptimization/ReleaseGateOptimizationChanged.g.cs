namespace AtlasOps.Features.Delivery.ReleaseGateOptimization;

public sealed record ReleaseGateOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);