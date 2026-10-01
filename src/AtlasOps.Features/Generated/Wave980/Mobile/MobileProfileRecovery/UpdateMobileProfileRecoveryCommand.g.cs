namespace AtlasOps.Features.Mobile.MobileProfileRecovery;

public sealed record UpdateMobileProfileRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);