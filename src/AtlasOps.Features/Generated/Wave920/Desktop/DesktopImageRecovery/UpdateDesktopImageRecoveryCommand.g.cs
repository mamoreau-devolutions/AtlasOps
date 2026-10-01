namespace AtlasOps.Features.Desktop.DesktopImageRecovery;

public sealed record UpdateDesktopImageRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);