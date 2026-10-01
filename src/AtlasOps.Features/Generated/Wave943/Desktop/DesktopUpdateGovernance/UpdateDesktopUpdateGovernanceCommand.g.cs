namespace AtlasOps.Features.Desktop.DesktopUpdateGovernance;

public sealed record UpdateDesktopUpdateGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);