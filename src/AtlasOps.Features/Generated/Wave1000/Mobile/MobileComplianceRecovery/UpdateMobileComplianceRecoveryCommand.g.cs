namespace AtlasOps.Features.Mobile.MobileComplianceRecovery;

public sealed record UpdateMobileComplianceRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);