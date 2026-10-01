namespace AtlasOps.Features.Messaging.MessageRetentionMonitoring;

public sealed record MessageRetentionMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);