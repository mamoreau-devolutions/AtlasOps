namespace AtlasOps.Features.Identity.IdentityGroupGovernance;

public sealed record IdentityGroupGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);