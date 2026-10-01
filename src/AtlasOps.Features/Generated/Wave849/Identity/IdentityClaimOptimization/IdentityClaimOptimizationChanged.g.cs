namespace AtlasOps.Features.Identity.IdentityClaimOptimization;

public sealed record IdentityClaimOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);