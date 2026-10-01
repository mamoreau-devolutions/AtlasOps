namespace AtlasOps.Features.Edge.EdgeDeploymentMonitoring;

public sealed record EdgeDeploymentMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);