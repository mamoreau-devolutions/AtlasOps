namespace AtlasOps.Features.Mobile.MobileFleetGovernance;

public sealed record UpdateMobileFleetGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);