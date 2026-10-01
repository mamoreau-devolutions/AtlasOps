namespace AtlasOps.Features.Database.DatabaseQueryOptimization;

public sealed record DatabaseQueryOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);