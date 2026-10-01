namespace AtlasOps.Features.Storage.ObjectBucketGovernance;

public sealed record ObjectBucketGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);