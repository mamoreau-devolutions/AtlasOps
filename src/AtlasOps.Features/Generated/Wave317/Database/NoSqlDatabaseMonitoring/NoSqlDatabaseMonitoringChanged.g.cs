namespace AtlasOps.Features.Database.NoSqlDatabaseMonitoring;

public sealed record NoSqlDatabaseMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);