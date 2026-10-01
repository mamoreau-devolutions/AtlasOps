namespace AtlasOps.Features.Api.ApiDeploymentMonitoring;

public sealed record ApiDeploymentMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);