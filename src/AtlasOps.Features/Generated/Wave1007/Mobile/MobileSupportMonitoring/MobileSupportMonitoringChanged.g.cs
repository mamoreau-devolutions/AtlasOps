namespace AtlasOps.Features.Mobile.MobileSupportMonitoring;

public sealed record MobileSupportMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);