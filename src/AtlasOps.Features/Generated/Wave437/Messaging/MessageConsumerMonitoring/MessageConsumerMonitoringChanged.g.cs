namespace AtlasOps.Features.Messaging.MessageConsumerMonitoring;

public sealed record MessageConsumerMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);