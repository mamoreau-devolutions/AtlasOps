namespace AtlasOps.Features.BusinessContinuity.RecoverySiteMonitoring;

public sealed record RecoverySiteMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);