namespace AtlasOps.Features.Data.DataTransformMonitoring;

public sealed record DataTransformMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);