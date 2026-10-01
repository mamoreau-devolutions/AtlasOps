namespace AtlasOps.Features.Storage.StorageArchiveOptimization;

public sealed record StorageArchiveOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);