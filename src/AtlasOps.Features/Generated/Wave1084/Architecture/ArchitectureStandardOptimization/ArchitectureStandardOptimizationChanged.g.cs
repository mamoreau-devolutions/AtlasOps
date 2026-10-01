namespace AtlasOps.Features.Architecture.ArchitectureStandardOptimization;

public sealed record ArchitectureStandardOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);