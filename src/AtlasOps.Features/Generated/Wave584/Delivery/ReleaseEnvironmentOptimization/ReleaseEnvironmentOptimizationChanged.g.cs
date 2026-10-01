namespace AtlasOps.Features.Delivery.ReleaseEnvironmentOptimization;

public sealed record ReleaseEnvironmentOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);