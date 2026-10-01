namespace AtlasOps.Features.Security.SecurityExceptionRecovery;

public sealed record SecurityExceptionRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);