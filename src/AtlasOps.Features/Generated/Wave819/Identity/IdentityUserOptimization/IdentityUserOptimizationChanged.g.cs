namespace AtlasOps.Features.Identity.IdentityUserOptimization;

public sealed record IdentityUserOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);