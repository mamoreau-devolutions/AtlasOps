namespace AtlasOps.Features.Security.SecurityScanGovernance;

public sealed record SecurityScanGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);