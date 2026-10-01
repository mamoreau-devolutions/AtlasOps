namespace AtlasOps.Features.Identity.IdentitySessionRecovery;

public sealed record IdentitySessionRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);