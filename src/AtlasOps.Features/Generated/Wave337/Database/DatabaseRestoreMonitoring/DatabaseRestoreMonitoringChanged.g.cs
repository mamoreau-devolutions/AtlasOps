namespace AtlasOps.Features.Database.DatabaseRestoreMonitoring;

public sealed record DatabaseRestoreMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);