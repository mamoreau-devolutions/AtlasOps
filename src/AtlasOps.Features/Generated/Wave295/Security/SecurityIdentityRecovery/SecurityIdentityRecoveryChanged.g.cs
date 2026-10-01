namespace AtlasOps.Features.Security.SecurityIdentityRecovery;

public sealed record SecurityIdentityRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);