namespace AtlasOps.Features.Database.DatabaseIndexOptimization;

public sealed record DatabaseIndexOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);