namespace AtlasOps.Features.Desktop.DesktopSessionRecovery;

public sealed record UpdateDesktopSessionRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);