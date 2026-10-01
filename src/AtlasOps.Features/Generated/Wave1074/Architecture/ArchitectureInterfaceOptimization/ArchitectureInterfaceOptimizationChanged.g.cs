namespace AtlasOps.Features.Architecture.ArchitectureInterfaceOptimization;

public sealed record ArchitectureInterfaceOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);