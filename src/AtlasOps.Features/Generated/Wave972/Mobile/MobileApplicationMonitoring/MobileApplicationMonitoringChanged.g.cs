namespace AtlasOps.Features.Mobile.MobileApplicationMonitoring;

public sealed record MobileApplicationMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);