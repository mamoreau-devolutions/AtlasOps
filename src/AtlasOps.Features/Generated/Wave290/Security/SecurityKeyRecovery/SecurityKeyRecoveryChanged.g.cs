namespace AtlasOps.Features.Security.SecurityKeyRecovery;

public sealed record SecurityKeyRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);