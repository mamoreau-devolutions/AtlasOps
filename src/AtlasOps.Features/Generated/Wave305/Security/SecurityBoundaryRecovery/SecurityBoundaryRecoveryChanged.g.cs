namespace AtlasOps.Features.Security.SecurityBoundaryRecovery;

public sealed record SecurityBoundaryRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);