namespace AtlasOps.Features.Security.SecuritySessionRecovery;

public sealed record SecuritySessionRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);