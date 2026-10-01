namespace AtlasOps.Features.Desktop.DesktopPeripheralRecovery;

public sealed record UpdateDesktopPeripheralRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);