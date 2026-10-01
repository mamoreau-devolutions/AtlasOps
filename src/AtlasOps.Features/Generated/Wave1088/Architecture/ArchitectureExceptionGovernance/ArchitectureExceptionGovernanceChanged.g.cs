namespace AtlasOps.Features.Architecture.ArchitectureExceptionGovernance;

public sealed record ArchitectureExceptionGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);