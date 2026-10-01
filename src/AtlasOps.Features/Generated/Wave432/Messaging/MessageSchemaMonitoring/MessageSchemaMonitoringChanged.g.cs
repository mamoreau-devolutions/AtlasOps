namespace AtlasOps.Features.Messaging.MessageSchemaMonitoring;

public sealed record MessageSchemaMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);