namespace AtlasOps.Features.Messaging.MessageSubscriptionMonitoring;

public sealed record MessageSubscriptionMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);