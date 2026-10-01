namespace AtlasOps.Features.Security.SecurityFindingRecovery;

public sealed record SecurityFindingRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);