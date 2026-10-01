namespace AtlasOps.Features.Mobile.MobileComplianceMonitoring;

public sealed record MobileComplianceMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);