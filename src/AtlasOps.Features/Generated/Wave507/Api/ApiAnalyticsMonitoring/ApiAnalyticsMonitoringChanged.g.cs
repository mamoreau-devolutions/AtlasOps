namespace AtlasOps.Features.Api.ApiAnalyticsMonitoring;

public sealed record ApiAnalyticsMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);