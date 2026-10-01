namespace AtlasOps.Features.Data.DataQualityMonitoring;

public sealed record DataQualityMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);