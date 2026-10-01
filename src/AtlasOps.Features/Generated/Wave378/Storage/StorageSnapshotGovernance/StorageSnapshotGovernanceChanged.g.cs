namespace AtlasOps.Features.Storage.StorageSnapshotGovernance;

public sealed record StorageSnapshotGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);