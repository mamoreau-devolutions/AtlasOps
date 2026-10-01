namespace AtlasOps.Features.Data.DataLineageMonitoring;

public sealed record DataLineageMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);