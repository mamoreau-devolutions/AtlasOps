namespace AtlasOps.Features.Storage.StorageLifecycleGovernance;

public sealed record StorageLifecycleGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);