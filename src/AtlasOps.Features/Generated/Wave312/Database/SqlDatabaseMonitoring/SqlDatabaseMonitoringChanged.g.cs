namespace AtlasOps.Features.Database.SqlDatabaseMonitoring;

public sealed record SqlDatabaseMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);