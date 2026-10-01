namespace AtlasOps.Features.Identity.IdentityRoleOptimization;

public sealed record IdentityRoleOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);