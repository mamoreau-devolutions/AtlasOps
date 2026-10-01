namespace AtlasOps.Features.Architecture.ArchitectureComponentOptimization;

public sealed record ArchitectureComponentOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);