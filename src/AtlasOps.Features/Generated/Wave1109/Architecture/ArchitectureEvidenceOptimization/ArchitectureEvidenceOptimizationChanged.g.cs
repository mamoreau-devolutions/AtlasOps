namespace AtlasOps.Features.Architecture.ArchitectureEvidenceOptimization;

public sealed record ArchitectureEvidenceOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);