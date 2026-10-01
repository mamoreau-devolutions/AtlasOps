namespace AtlasOps.Features.Data.DataContractOptimization;

public sealed record DataContractOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);