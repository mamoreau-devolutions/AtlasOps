namespace AtlasOps.Features.Data.DataDatasetMonitoring;

public sealed record DataDatasetMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);