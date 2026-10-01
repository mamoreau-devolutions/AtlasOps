namespace AtlasOps.Features.Identity.IdentityAuditRecovery;

public sealed record IdentityAuditRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);