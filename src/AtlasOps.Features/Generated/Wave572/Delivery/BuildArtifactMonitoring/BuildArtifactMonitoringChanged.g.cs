namespace AtlasOps.Features.Delivery.BuildArtifactMonitoring;

public sealed record BuildArtifactMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);