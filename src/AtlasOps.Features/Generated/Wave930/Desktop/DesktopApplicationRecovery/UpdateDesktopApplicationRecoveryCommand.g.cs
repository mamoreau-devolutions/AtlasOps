namespace AtlasOps.Features.Desktop.DesktopApplicationRecovery;

public sealed record UpdateDesktopApplicationRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);