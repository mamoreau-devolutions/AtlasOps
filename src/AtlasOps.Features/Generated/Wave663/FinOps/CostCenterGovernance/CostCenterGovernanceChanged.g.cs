namespace AtlasOps.Features.FinOps.CostCenterGovernance;

public sealed record CostCenterGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);