namespace AtlasOps.Features.Desktop.DesktopPolicyRecovery;

public sealed record UpdateDesktopPolicyRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);