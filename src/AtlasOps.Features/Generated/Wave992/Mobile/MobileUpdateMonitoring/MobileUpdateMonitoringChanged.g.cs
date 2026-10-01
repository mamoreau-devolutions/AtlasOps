namespace AtlasOps.Features.Mobile.MobileUpdateMonitoring;

public sealed record MobileUpdateMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);