namespace AtlasOps.Features.Architecture.ArchitectureInterfaceGovernance;

public sealed record ArchitectureInterfaceGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);