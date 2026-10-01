namespace AtlasOps.Features.Identity.IdentityAuditGovernance;

public sealed record IdentityAuditGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);