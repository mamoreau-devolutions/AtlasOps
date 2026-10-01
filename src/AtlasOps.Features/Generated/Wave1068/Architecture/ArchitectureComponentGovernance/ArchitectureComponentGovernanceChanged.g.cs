namespace AtlasOps.Features.Architecture.ArchitectureComponentGovernance;

public sealed record ArchitectureComponentGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);