namespace AtlasOps.Features.Identity.IdentityUserGovernance;

public sealed record IdentityUserGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);