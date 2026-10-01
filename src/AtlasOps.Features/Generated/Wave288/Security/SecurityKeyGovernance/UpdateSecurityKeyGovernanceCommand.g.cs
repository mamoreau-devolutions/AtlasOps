namespace AtlasOps.Features.Security.SecurityKeyGovernance;

public sealed record UpdateSecurityKeyGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);