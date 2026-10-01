namespace AtlasOps.Features.Architecture.ArchitectureRoadmapRecovery;

public sealed record ArchitectureRoadmapRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);