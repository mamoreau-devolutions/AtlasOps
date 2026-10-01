namespace AtlasOps.Features.Identity.IdentityProviderOptimization;

public sealed record IdentityProviderOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);