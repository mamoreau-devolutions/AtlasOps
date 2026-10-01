namespace AtlasOps.Features.Mobile.MobileApplicationGovernance;

public sealed record UpdateMobileApplicationGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);