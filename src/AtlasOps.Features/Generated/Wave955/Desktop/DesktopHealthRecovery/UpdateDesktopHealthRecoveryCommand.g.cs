namespace AtlasOps.Features.Desktop.DesktopHealthRecovery;

public sealed record UpdateDesktopHealthRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);