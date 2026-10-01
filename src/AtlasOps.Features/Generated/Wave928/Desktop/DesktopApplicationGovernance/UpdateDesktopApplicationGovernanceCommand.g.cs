namespace AtlasOps.Features.Desktop.DesktopApplicationGovernance;

public sealed record UpdateDesktopApplicationGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);