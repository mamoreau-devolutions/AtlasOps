namespace AtlasOps.Features.Identity.IdentityRoleRecovery;

public sealed record IdentityRoleRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);