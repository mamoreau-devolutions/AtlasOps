namespace AtlasOps.Features.Security.SecurityCertificateMonitoring;

public sealed record SecurityCertificateMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);