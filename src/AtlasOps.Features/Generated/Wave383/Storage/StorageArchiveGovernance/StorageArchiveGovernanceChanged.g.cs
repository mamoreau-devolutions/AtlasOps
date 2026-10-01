namespace AtlasOps.Features.Storage.StorageArchiveGovernance;

public sealed record StorageArchiveGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);