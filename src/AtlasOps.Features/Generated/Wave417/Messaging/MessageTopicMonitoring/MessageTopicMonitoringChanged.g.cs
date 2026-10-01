namespace AtlasOps.Features.Messaging.MessageTopicMonitoring;

public sealed record MessageTopicMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);