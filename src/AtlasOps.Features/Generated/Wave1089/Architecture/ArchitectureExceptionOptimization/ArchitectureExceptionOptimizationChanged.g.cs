namespace AtlasOps.Features.Architecture.ArchitectureExceptionOptimization;

public sealed record ArchitectureExceptionOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);