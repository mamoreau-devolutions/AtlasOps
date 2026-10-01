namespace AtlasOps.Features.Identity.IdentityProviderGovernance;

public sealed record IdentityProviderGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);