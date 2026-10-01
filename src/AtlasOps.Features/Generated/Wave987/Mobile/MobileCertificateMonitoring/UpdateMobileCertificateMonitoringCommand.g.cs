namespace AtlasOps.Features.Mobile.MobileCertificateMonitoring;

public sealed record UpdateMobileCertificateMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);