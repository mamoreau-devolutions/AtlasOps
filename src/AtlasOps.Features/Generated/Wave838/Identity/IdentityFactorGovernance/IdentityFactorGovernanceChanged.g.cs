namespace AtlasOps.Features.Identity.IdentityFactorGovernance;

public sealed record IdentityFactorGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);