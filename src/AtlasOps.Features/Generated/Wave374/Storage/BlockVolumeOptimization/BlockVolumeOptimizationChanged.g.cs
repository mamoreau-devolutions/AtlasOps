namespace AtlasOps.Features.Storage.BlockVolumeOptimization;

public sealed record BlockVolumeOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);