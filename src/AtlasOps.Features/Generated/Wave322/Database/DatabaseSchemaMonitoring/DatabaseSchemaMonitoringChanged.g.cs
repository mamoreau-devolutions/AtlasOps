namespace AtlasOps.Features.Database.DatabaseSchemaMonitoring;

public sealed record DatabaseSchemaMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);