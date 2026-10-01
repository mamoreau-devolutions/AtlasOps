namespace AtlasOps.Features.Api.ApiContractOptimization;

public sealed record ApiContractOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);