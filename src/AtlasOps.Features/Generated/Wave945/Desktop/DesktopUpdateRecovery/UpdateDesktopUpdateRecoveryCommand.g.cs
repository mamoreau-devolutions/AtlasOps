namespace AtlasOps.Features.Desktop.DesktopUpdateRecovery;

public sealed record UpdateDesktopUpdateRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);