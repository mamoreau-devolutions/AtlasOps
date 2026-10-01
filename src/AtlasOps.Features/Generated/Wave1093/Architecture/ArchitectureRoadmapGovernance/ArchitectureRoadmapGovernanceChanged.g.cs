namespace AtlasOps.Features.Architecture.ArchitectureRoadmapGovernance;

public sealed record ArchitectureRoadmapGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);