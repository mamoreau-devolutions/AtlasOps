namespace AtlasOps.Features.Mobile.MobileDeviceMonitoring;

public sealed record MobileDeviceMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);