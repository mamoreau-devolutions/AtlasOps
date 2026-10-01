namespace AtlasOps.Features.Storage.FileShareOptimization;

public sealed record FileShareOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);