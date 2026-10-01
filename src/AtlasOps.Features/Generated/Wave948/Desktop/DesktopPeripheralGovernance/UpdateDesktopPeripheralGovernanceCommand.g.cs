namespace AtlasOps.Features.Desktop.DesktopPeripheralGovernance;

public sealed record UpdateDesktopPeripheralGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);