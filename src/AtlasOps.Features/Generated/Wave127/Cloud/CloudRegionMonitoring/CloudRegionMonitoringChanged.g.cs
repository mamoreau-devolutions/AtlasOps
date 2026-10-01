namespace AtlasOps.Features.Cloud.CloudRegionMonitoring;

public sealed record CloudRegionMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);