namespace AtlasOps.Features.Mobile.MobileApplicationRecovery;

public sealed record UpdateMobileApplicationRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);