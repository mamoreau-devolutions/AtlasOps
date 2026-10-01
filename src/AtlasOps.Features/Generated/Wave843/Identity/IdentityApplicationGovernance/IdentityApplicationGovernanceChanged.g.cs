namespace AtlasOps.Features.Identity.IdentityApplicationGovernance;

public sealed record IdentityApplicationGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);