namespace AtlasOps.Features.Security.SecurityPatchRecovery;

public sealed record SecurityPatchRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);