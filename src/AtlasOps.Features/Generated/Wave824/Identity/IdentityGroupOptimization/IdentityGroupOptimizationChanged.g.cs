namespace AtlasOps.Features.Identity.IdentityGroupOptimization;

public sealed record IdentityGroupOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);