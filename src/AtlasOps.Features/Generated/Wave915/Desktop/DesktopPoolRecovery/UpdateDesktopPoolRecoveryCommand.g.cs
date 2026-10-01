namespace AtlasOps.Features.Desktop.DesktopPoolRecovery;

public sealed record UpdateDesktopPoolRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);