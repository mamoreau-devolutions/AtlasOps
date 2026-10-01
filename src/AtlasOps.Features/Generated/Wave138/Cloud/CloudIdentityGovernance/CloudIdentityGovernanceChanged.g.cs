namespace AtlasOps.Features.Cloud.CloudIdentityGovernance;

public sealed record CloudIdentityGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);