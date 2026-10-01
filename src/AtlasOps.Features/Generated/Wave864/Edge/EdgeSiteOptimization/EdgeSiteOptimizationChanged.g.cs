namespace AtlasOps.Features.Edge.EdgeSiteOptimization;

public sealed record EdgeSiteOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);