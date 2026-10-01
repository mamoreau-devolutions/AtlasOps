namespace AtlasOps.Features.Mobile.MobileDeviceGovernance;

public sealed record UpdateMobileDeviceGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);