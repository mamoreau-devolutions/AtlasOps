namespace AtlasOps.Features.Identity.IdentityGroupRecovery;

public sealed record IdentityGroupRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);