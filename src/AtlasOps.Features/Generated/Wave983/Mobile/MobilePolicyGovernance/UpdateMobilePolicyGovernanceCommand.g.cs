namespace AtlasOps.Features.Mobile.MobilePolicyGovernance;

public sealed record UpdateMobilePolicyGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);