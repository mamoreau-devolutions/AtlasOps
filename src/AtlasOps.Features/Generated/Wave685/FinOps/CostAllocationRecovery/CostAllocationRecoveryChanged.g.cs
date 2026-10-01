namespace AtlasOps.Features.FinOps.CostAllocationRecovery;

public sealed record CostAllocationRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);