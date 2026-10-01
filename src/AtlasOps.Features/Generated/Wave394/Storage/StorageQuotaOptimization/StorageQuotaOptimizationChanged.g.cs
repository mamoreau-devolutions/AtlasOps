namespace AtlasOps.Features.Storage.StorageQuotaOptimization;

public sealed record StorageQuotaOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);