namespace AtlasOps.Features.Storage.StorageTransferGovernance;

public sealed record StorageTransferGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);