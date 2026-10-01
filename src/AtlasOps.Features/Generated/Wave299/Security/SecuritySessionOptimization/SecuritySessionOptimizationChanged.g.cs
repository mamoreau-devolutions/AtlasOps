namespace AtlasOps.Features.Security.SecuritySessionOptimization;

public sealed record SecuritySessionOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);