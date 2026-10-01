namespace AtlasOps.Features.FinOps.CostAllocationGovernance;

public sealed record CostAllocationGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);