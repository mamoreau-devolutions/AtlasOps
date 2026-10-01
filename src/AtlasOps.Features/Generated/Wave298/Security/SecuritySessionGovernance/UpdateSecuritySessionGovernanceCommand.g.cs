namespace AtlasOps.Features.Security.SecuritySessionGovernance;

public sealed record UpdateSecuritySessionGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);