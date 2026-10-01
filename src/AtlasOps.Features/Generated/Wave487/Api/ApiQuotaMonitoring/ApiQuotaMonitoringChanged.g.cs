namespace AtlasOps.Features.Api.ApiQuotaMonitoring;

public sealed record ApiQuotaMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);