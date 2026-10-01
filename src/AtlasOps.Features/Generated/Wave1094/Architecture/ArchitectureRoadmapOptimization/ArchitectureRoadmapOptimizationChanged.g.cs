namespace AtlasOps.Features.Architecture.ArchitectureRoadmapOptimization;

public sealed record ArchitectureRoadmapOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);