namespace AtlasOps.Features.Cloud.CloudDatabaseOptimization;

public sealed record CloudDatabaseOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);