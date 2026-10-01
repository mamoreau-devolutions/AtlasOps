namespace AtlasOps.Features.Architecture.ArchitectureDecisionOptimization;

public sealed record ArchitectureDecisionOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);