namespace AtlasOps.Features.Identity.IdentityApplicationOptimization;

public sealed record IdentityApplicationOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);