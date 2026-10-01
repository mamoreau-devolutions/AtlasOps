namespace AtlasOps.Features.Database.DatabaseSchemaOptimization;

public sealed record DatabaseSchemaOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);