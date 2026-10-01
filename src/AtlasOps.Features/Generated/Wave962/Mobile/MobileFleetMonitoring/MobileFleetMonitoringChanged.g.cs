namespace AtlasOps.Features.Mobile.MobileFleetMonitoring;

public sealed record MobileFleetMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);