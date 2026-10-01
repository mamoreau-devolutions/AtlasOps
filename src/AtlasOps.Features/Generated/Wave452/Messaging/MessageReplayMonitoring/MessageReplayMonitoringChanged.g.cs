namespace AtlasOps.Features.Messaging.MessageReplayMonitoring;

public sealed record MessageReplayMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);