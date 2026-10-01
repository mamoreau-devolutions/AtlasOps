namespace AtlasOps.Features.Messaging.MessageBrokerOptimization;

public sealed record MessageBrokerOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);