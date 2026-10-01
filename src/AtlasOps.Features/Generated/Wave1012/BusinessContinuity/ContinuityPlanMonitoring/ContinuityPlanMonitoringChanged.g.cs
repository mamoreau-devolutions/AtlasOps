namespace AtlasOps.Features.BusinessContinuity.ContinuityPlanMonitoring;

public sealed record ContinuityPlanMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);