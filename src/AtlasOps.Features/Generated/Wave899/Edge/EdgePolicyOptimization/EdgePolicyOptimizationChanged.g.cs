namespace AtlasOps.Features.Edge.EdgePolicyOptimization;

public sealed record EdgePolicyOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);