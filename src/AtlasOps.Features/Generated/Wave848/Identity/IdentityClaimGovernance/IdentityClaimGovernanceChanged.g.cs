namespace AtlasOps.Features.Identity.IdentityClaimGovernance;

public sealed record IdentityClaimGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);