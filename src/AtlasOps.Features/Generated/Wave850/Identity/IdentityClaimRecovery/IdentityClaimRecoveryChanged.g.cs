namespace AtlasOps.Features.Identity.IdentityClaimRecovery;

public sealed record IdentityClaimRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);