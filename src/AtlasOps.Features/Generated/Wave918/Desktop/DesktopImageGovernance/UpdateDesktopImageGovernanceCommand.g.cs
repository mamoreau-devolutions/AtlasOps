namespace AtlasOps.Features.Desktop.DesktopImageGovernance;

public sealed record UpdateDesktopImageGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);