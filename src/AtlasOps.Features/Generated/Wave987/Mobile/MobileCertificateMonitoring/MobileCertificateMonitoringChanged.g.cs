namespace AtlasOps.Features.Mobile.MobileCertificateMonitoring;

public sealed record MobileCertificateMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);