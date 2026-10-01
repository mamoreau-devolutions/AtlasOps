namespace AtlasOps.Features.Edge.EdgeUpdateOptimization;

public sealed record EdgeUpdateOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);