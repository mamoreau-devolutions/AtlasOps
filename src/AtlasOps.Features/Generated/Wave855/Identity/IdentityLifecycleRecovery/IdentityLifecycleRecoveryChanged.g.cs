namespace AtlasOps.Features.Identity.IdentityLifecycleRecovery;

public sealed record IdentityLifecycleRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);