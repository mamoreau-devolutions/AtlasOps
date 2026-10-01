namespace AtlasOps.Features.Desktop.DesktopPolicyGovernance;

public sealed record UpdateDesktopPolicyGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);