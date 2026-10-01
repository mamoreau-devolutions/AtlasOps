namespace AtlasOps.Features.Cloud.CloudStorageMonitoring;

public sealed record CloudStorageMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);