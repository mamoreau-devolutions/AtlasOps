namespace AtlasOps.Features.Data.DataAccessMonitoring;

public sealed record DataAccessMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);