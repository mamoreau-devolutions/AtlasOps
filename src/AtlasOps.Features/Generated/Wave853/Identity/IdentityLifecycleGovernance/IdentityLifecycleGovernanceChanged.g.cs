namespace AtlasOps.Features.Identity.IdentityLifecycleGovernance;

public sealed record IdentityLifecycleGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);