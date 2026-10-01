namespace AtlasOps.Features.Desktop.DesktopHealthGovernance;

public sealed record UpdateDesktopHealthGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);