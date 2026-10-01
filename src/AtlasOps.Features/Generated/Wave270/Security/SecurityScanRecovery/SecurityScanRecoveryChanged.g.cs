namespace AtlasOps.Features.Security.SecurityScanRecovery;

public sealed record SecurityScanRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);