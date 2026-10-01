namespace AtlasOps.Features.Security.SecurityBaselineRecovery;

public sealed record SecurityBaselineRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);