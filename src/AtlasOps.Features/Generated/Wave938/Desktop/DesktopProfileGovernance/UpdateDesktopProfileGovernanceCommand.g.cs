namespace AtlasOps.Features.Desktop.DesktopProfileGovernance;

public sealed record UpdateDesktopProfileGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);