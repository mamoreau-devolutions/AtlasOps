namespace AtlasOps.Features.Cloud.CloudStorageGovernance;

public sealed record CloudStorageGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);