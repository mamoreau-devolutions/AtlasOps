namespace AtlasOps.Features.Mobile.MobileCertificateRecovery;

public sealed record UpdateMobileCertificateRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);