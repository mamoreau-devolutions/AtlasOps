namespace AtlasOps.Features.Mobile.MobileSupportRecovery;

public sealed record UpdateMobileSupportRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);