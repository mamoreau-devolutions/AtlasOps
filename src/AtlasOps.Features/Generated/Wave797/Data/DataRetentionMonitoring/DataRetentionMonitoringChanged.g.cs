namespace AtlasOps.Features.Data.DataRetentionMonitoring;

public sealed record DataRetentionMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);