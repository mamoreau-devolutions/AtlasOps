namespace AtlasOps.Features.Database.DatabaseReplicaOptimization;

public sealed record DatabaseReplicaOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);