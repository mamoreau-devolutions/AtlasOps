namespace AtlasOps.Features.FinOps.ResourceCommitmentGovernance;

public sealed record ResourceCommitmentGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);