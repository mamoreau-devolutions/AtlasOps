namespace AtlasOps.Features.Mobile.MobilePolicyMonitoring;

public sealed record MobilePolicyMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);