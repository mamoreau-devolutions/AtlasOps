namespace AtlasOps.Features.Mobile.MobileTelemetryMonitoring;

public sealed record MobileTelemetryMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);