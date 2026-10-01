namespace AtlasOps.Features.Mobile.MobileProfileGovernance;

public sealed record UpdateMobileProfileGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);