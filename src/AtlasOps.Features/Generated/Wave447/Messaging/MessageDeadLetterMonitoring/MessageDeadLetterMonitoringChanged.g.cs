namespace AtlasOps.Features.Messaging.MessageDeadLetterMonitoring;

public sealed record MessageDeadLetterMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);