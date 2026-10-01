namespace AtlasOps.Features.Messaging.MessageProducerMonitoring;

public sealed record MessageProducerMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);