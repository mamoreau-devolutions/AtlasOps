namespace AtlasOps.Features.FinOps.CostCenterRecovery;

public sealed record CostCenterRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);