namespace AtlasOps.Features.Desktop.DesktopLicenseGovernance;

public sealed record UpdateDesktopLicenseGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);