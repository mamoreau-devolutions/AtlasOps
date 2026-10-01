namespace AtlasOps.Features.Architecture.ArchitectureDecisionGovernance;

public sealed record ArchitectureDecisionGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);