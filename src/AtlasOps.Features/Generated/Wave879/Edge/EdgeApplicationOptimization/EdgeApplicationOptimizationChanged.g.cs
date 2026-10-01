namespace AtlasOps.Features.Edge.EdgeApplicationOptimization;

public sealed record EdgeApplicationOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);