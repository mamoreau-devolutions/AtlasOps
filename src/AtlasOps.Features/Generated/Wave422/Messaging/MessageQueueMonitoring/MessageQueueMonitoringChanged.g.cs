namespace AtlasOps.Features.Messaging.MessageQueueMonitoring;

public sealed record MessageQueueMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);