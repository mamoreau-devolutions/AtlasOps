namespace AtlasOps.Features.Messaging.MessageSchemaOptimization;

public sealed record MessageSchemaOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);