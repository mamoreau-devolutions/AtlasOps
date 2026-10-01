namespace AtlasOps.Features.Database.DatabaseBackupMonitoring;

public sealed record DatabaseBackupMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);