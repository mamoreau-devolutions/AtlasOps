namespace AtlasOps.Features.Data.DataPipelineMonitoring;

public sealed record DataPipelineMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);