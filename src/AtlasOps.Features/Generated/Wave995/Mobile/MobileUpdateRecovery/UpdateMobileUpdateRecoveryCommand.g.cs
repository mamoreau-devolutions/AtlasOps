namespace AtlasOps.Features.Mobile.MobileUpdateRecovery;

public sealed record UpdateMobileUpdateRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);