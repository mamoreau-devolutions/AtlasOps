namespace AtlasOps.Features.Storage.StorageQuotaRecovery;

public sealed record StorageQuotaRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);