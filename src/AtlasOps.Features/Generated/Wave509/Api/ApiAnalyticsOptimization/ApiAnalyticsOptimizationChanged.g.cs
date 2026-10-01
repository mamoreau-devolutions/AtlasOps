namespace AtlasOps.Features.Api.ApiAnalyticsOptimization;

public sealed record ApiAnalyticsOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);