namespace AtlasOps.Features.Api.ApiQuotaOptimization;

public sealed record ApiQuotaOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);