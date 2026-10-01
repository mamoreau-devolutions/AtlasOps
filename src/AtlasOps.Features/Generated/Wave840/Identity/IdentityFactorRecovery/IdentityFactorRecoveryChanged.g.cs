namespace AtlasOps.Features.Identity.IdentityFactorRecovery;

public sealed record IdentityFactorRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);