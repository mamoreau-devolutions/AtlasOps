namespace AtlasOps.Features.Messaging.MessageBrokerMonitoring;

public sealed record MessageBrokerMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);