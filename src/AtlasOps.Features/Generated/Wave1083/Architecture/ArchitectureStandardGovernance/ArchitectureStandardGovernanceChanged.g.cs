namespace AtlasOps.Features.Architecture.ArchitectureStandardGovernance;

public sealed record ArchitectureStandardGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);