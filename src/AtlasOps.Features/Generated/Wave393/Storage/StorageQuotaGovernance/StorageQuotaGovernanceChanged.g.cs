namespace AtlasOps.Features.Storage.StorageQuotaGovernance;

public sealed record StorageQuotaGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);