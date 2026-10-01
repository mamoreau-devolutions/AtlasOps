namespace AtlasOps.Features.Architecture.ArchitectureRiskGovernance;

public sealed record ArchitectureRiskGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);