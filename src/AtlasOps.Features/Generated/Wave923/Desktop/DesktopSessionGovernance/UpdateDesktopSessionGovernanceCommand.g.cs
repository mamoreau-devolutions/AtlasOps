namespace AtlasOps.Features.Desktop.DesktopSessionGovernance;

public sealed record UpdateDesktopSessionGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);