namespace AtlasOps.Features.Identity.IdentitySessionOptimization;

public sealed record IdentitySessionOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);