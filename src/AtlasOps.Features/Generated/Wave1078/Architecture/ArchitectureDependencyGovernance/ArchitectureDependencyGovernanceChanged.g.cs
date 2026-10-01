namespace AtlasOps.Features.Architecture.ArchitectureDependencyGovernance;

public sealed record ArchitectureDependencyGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);