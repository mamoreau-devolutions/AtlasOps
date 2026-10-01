namespace AtlasOps.Features.Security.SecurityPatchGovernance;

public sealed record SecurityPatchGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);