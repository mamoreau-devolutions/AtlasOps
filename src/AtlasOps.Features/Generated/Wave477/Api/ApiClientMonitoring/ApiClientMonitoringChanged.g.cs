namespace AtlasOps.Features.Api.ApiClientMonitoring;

public sealed record ApiClientMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);