namespace AtlasOps.Features.Mobile.MobileCertificateGovernance;

public sealed record UpdateMobileCertificateGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);