namespace AtlasOps.Features.Mobile.MobileUpdateGovernance;

public sealed record UpdateMobileUpdateGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);