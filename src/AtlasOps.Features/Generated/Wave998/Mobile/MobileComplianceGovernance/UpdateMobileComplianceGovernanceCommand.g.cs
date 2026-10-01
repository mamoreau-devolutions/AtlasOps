namespace AtlasOps.Features.Mobile.MobileComplianceGovernance;

public sealed record UpdateMobileComplianceGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);