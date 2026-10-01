namespace AtlasOps.Features.Identity.IdentityRoleGovernance;

public sealed record IdentityRoleGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);