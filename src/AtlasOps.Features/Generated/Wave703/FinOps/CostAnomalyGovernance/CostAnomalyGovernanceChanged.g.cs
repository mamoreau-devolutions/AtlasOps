namespace AtlasOps.Features.FinOps.CostAnomalyGovernance;

public sealed record CostAnomalyGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);