namespace AtlasOps.Features.Mobile.MobilePolicyRecovery;

public sealed record UpdateMobilePolicyRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);