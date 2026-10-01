namespace AtlasOps.Features.Desktop.DesktopProfileRecovery;

public sealed record UpdateDesktopProfileRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);