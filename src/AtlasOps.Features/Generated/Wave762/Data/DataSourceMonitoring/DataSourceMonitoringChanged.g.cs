namespace AtlasOps.Features.Data.DataSourceMonitoring;

public sealed record DataSourceMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);