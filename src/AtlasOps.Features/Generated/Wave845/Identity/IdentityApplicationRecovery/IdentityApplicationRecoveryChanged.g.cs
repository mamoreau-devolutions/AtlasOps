namespace AtlasOps.Features.Identity.IdentityApplicationRecovery;

public sealed record IdentityApplicationRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);