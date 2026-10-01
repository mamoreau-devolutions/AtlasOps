namespace AtlasOps.Features.Mobile.MobileSupportGovernance;

public sealed record UpdateMobileSupportGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);