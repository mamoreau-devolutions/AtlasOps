namespace AtlasOps.Features.Identity.IdentityFactorOptimization;

public sealed record IdentityFactorOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);