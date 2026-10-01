namespace AtlasOps.Features.Identity.IdentityAuditOptimization;

public sealed record IdentityAuditOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);