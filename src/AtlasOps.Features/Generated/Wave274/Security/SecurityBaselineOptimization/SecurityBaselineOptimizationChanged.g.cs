namespace AtlasOps.Features.Security.SecurityBaselineOptimization;

public sealed record SecurityBaselineOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);