namespace AtlasOps.Features.Storage.StorageReplicationGovernance;

public sealed record StorageReplicationGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);