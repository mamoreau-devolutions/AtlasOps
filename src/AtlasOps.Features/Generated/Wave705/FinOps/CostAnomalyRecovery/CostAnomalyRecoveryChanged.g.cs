namespace AtlasOps.Features.FinOps.CostAnomalyRecovery;

public sealed record CostAnomalyRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);