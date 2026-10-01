namespace AtlasOps.Features.Storage.StorageEncryptionGovernance;

public sealed record StorageEncryptionGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);