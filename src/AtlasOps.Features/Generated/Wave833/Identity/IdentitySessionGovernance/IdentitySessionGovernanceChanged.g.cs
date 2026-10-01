namespace AtlasOps.Features.Identity.IdentitySessionGovernance;

public sealed record IdentitySessionGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);