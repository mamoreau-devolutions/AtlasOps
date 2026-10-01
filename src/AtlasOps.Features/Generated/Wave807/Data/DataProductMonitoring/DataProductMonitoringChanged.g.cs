namespace AtlasOps.Features.Data.DataProductMonitoring;

public sealed record DataProductMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);