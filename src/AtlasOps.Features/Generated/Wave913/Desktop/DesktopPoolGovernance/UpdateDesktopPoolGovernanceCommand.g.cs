namespace AtlasOps.Features.Desktop.DesktopPoolGovernance;

public sealed record UpdateDesktopPoolGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);