namespace AtlasOps.Features.Architecture.ArchitectureRiskOptimization;

public sealed record ArchitectureRiskOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);