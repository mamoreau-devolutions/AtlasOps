namespace AtlasOps.Features.Mobile.MobileProfileMonitoring;

public sealed record MobileProfileMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);