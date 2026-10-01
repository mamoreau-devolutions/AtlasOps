namespace AtlasOps.Features.Identity.IdentityProviderRecovery;

public sealed record IdentityProviderRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);