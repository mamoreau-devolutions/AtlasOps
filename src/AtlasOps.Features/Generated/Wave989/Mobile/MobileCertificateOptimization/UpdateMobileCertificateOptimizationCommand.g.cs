namespace AtlasOps.Features.Mobile.MobileCertificateOptimization;

public sealed record UpdateMobileCertificateOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);