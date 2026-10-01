namespace AtlasOps.Features.Mobile.MobileDeviceRecovery;

public sealed record UpdateMobileDeviceRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);