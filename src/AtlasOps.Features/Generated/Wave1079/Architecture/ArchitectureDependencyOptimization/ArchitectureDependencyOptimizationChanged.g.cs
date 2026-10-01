namespace AtlasOps.Features.Architecture.ArchitectureDependencyOptimization;

public sealed record ArchitectureDependencyOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);