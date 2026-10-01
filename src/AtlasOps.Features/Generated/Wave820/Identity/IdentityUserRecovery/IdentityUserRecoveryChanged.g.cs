namespace AtlasOps.Features.Identity.IdentityUserRecovery;

public sealed record IdentityUserRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);